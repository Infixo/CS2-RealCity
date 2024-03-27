using System;
using System.Reflection;
using System.Collections.Generic;
using Unity.Entities;
using Game.Prefabs;
using Game.Economy;

namespace RealCity.Config;

public static class ConfigTool
{
    private static PrefabSystem m_PrefabSystem;
    private static EntityManager m_EntityManager;

    public static void DumpFields(PrefabBase prefab, ComponentBase component)
    {
        string className = component.GetType().Name;
        Mod.log.Info($"{prefab.name}.{component.name}.CLASS: {className}");

        object obj = (object)component;
        Type type = obj.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // field components: System.Collections.Generic.List`1[Game.Prefabs.ComponentBase]
            if (field.Name != "isDirty" && field.Name != "active" && field.Name != "components")
            {
                object value = field.GetValue(obj);
                Mod.log.Info($"{prefab.name}.{component.name}.{field.Name}: {value}");
            }
        }
    }

    // NOT USED ATM
    /// <summary>
    /// Configures a specific component withing a specific prefab according to config data.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="prefabConfig"></param>
    /// <param name="comp"></param>
    private static void ConfigureComponent(PrefabBase prefab, PrefabXml prefabConfig, ComponentBase component)
    {
        string compName = component.GetType().Name;

        // Structs within components are handled as separate components
        // TODO: When more structs are implemented, use Reflection to create a flexible code for all possible cases
        if (compName == "ProcessingCompany" && prefabConfig.TryGetComponent("IndustrialProcess", out ComponentXml structConfig))
        {
            // IndustrialProcess - currently 2 fields are supported
            ProcessingCompany comp = component as ProcessingCompany;
            IndustrialProcess oldProc = comp.process;
            if (structConfig.TryGetField("m_MaxWorkersPerCell", out FieldXml mwpcField) && mwpcField.ValueFloatSpecified)
            {
                comp.process.m_MaxWorkersPerCell = mwpcField.ValueFloat ?? oldProc.m_MaxWorkersPerCell;
                Mod.LogIf($"{prefab.name}.IndustrialProcess.{mwpcField.Name}: {oldProc.m_MaxWorkersPerCell} -> {comp.process.m_MaxWorkersPerCell} ({comp.process.m_MaxWorkersPerCell.GetType()}, {mwpcField})");
            }
            if (structConfig.TryGetField("m_Output.m_Amount", out FieldXml outamtField) && outamtField.ValueIntSpecified)
            {
                comp.process.m_Output.m_Amount = outamtField.ValueInt ?? oldProc.m_Output.m_Amount;
                Mod.LogIf($"{prefab.name}.IndustrialProcess.{outamtField.Name}: {oldProc.m_Output.m_Amount} -> {comp.process.m_Output.m_Amount} ({comp.process.m_Output.m_Amount.GetType()}, {outamtField})");
            }
            if (!Mod.setting.Logging)
                Mod.log.Info($"{prefab.name}.IndustrialProcess: wpc {comp.process.m_MaxWorkersPerCell} output {comp.process.m_Output.m_Amount}");
        }

        if (!prefabConfig.TryGetComponent(compName, out ComponentXml compConfig))
        {
            Mod.LogIf($"{prefab.name}.{compName}: SKIP");
            return;
        }

        Mod.LogIf($"{prefab.name}.{compName}: valid");
        foreach (FieldXml fieldConfig in compConfig.Fields)
        {
            // Get the FieldInfo object for the field with the given name
            FieldInfo field = component.GetType().GetField(fieldConfig.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                object oldValue = field.GetValue(component);

                // TODO: extend for other field types
                if (field.FieldType == typeof(float))
                {
                    field.SetValue(component, fieldConfig.ValueFloat);
                }
                else
                {
                    field.SetValue(component, fieldConfig.ValueInt);
                }
                if (Mod.setting.Logging)
                    Mod.log.Info($"{prefab.name}.{compName}.{field.Name}: {oldValue} -> {field.GetValue(component)} ({field.FieldType}, {fieldConfig})");
                else
                    Mod.log.Info($"{prefab.name}.{compName}.{field.Name}: {field.GetValue(component)}");
            }
            else
            {
                Mod.log.Info($"{prefab.name}.{compName}: Warning! Field {fieldConfig.Name} not found in the component.");
            }
        }
        if (Mod.setting.Logging) DumpFields(prefab, component); // debug
    }

    // Method to change the value of a field in an ECS component by name
    // NOT USED
    public static void SetFieldValue<T>(ref T component, string fieldName, object newValue) where T : struct, IComponentData
    {
        Type type = typeof(T);
        FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            object oldValue = field.GetValue(component);
            field.SetValueDirect(__makeref(component), newValue);
            Mod.log.Info($"{type.Name}.{field.Name}: {oldValue} -> {field.GetValue(component)} ({field.FieldType})");
        }
        else
        {
            Mod.log.Info($"Field '{fieldName}' not found in struct '{type.Name}'.");
        }
    }

    // NOT USED - CRASHES THE GAME ATM
    public static void ConfigureComponentData<T>(ComponentXml compXml, ref T component) where T : struct, IComponentData
    {
        Type type = typeof(T);
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            object oldValue = field.GetValue(component);
            // change it
            if (compXml.TryGetField(field.Name, out FieldXml fieldXml))
            {
                // TODO: extend for other field types
                if (field.FieldType == typeof(float))
                {
                    field.SetValueDirect(__makeref(component), fieldXml.ValueFloat);
                }
                else
                {
                    field.SetValueDirect(__makeref(component), fieldXml.ValueInt);
                }
                Mod.log.Info($"{type.Name}.{field.Name}: {oldValue} -> {field.GetValue(component)} ({field.FieldType})");
            }
            else
                Mod.LogIf($"{type.Name}.{field.Name}: {oldValue}");
        }
    }

    /// <summary>
    /// Configures a specific component within a specific prefab according to config data.
    /// </summary>
    /// <param name="compXml"></param>
    /// <param name="prefab"></param>
    /// <param name="entity"></param>
    private static void ConfigureComponent(ComponentXml compXml, PrefabBase prefab, Entity entity)
    {
        // Infixo: first version which is not yet dynamic :(
        FieldXml fieldXml;
        switch (compXml.Name)
        {
            case "WorkplaceData":
                if (m_PrefabSystem.TryGetComponentData<WorkplaceData>(prefab, out WorkplaceData workplaceData))
                {
                    if (compXml.TryGetField("m_Complexity", out fieldXml))
                    {
                        workplaceData.m_Complexity = (WorkplaceComplexity)fieldXml.ValueInt;
                    }
                    if (compXml.TryGetField("m_MaxWorkers", out fieldXml))
                    {
                        workplaceData.m_MaxWorkers = (int)fieldXml.ValueInt;
                    }
                    m_PrefabSystem.AddComponentData<WorkplaceData>(prefab, workplaceData);
                    Mod.log.Info($"{prefab.name}.{compXml.Name}: {workplaceData.m_Complexity} {workplaceData.m_MaxWorkers}");
                }
                break;
            case "DeathcareFacilityData":
                if (m_PrefabSystem.TryGetComponentData<DeathcareFacilityData>(prefab, out DeathcareFacilityData deathcareFacilityData))
                {
                    if (compXml.TryGetField("m_ProcessingRate", out fieldXml))
                    {
                        deathcareFacilityData.m_ProcessingRate = (float)fieldXml.ValueFloat;
                    }
                    m_PrefabSystem.AddComponentData<DeathcareFacilityData>(prefab, deathcareFacilityData);
                    Mod.log.Info($"{prefab.name}.{compXml.Name}: {deathcareFacilityData.m_ProcessingRate}");
                }
                break;
            case "PostFacilityData":
                if (m_PrefabSystem.TryGetComponentData<PostFacilityData>(prefab, out PostFacilityData postFacilityData))
                {
                    if (compXml.TryGetField("m_SortingRate", out fieldXml))
                    {
                        postFacilityData.m_SortingRate = (int)fieldXml.ValueInt;
                    }
                    m_PrefabSystem.AddComponentData<PostFacilityData>(prefab, postFacilityData);
                    Mod.log.Info($"{prefab.name}.{compXml.Name}: {postFacilityData.m_SortingRate}");
                }
                break;
            default:
                Mod.log.Warn($"{compXml} is not supported.");
                break;
        }
    }

    /// <summary>
    /// Configures a specific prefab according to the config data.
    /// </summary>
    /// <param name="prefabXml"></param>
    /// <param name="prefab"></param>
    /// <param name="entity"></param>
    private static void ConfigurePrefab(PrefabXml prefabXml, PrefabBase prefab, Entity entity)
    {
        Mod.LogIf($"{prefab.name}: valid {prefab.GetType().Name}");
        // iterate through components and see which ones need to be changed
        foreach (ComponentXml componentXml in prefabXml.Components)
            ConfigureComponent(componentXml, prefab, entity);
    }

    public static void Apply()
    {
        m_PrefabSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
        m_EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (PrefabXml prefabXml in ConfigToolXml.Config.Prefabs)
        {
            PrefabID prefabID = new PrefabID(prefabXml.Type, prefabXml.Name);
            if (m_PrefabSystem.TryGetPrefab(prefabID, out PrefabBase prefab) && m_PrefabSystem.TryGetEntity(prefab, out Entity entity))
            {
                if (ConfigToolXml.Config.IsPrefabValid(prefab.GetType().Name))
                {
                    ConfigurePrefab(prefabXml, prefab, entity);
                }
                else
                    Mod.log.Info($"{prefab.name}: SKIP {prefab.GetType().Name}");
            }
            else
                Mod.log.Warn($"Failed to retrieve {prefabXml} from the PrefabSystem.");
        }
    }

    // List components from entity
    internal static void ListComponents(PrefabBase prefab, Entity entity)
    {
        foreach (ComponentType componentType in m_EntityManager.GetComponentTypes(entity))
        {
            Mod.log.Info($"{prefab.GetType().Name}.{prefab.name}.{componentType.GetManagedType().Name}: {componentType}");
        }
    }
}

// FOR THE FUTURE
/* This code reads a dictionary and puts it into the config xml


ConfigurationXml config = ConfigToolXml.Config;

foreach (var item in PrefabSystem_AddPrefab_Patches.MaxWorkersPerCellDict)
{
    Plugin.Log($"DICT {item.Key} {item.Value}");
    PrefabXml prefabConfig = default(PrefabXml);
    if (!config.TryGetPrefab(item.Key, out prefabConfig))
        config.Prefabs.Add( new PrefabXml { Name = item.Key, Components = new List<ComponentXml>() });
    if (config.TryGetPrefab(item.Key, out prefabConfig))
    {
        ComponentXml compConfig = default(ComponentXml);
        if (!prefabConfig.TryGetComponent("IndustrialProcess", out compConfig))
            prefabConfig.Components.Add( new ComponentXml { Name = "IndustrialProcess", Fields = new List<FieldXml>() });
        if (prefabConfig.TryGetComponent("IndustrialProcess", out compConfig))
        {
            if (!compConfig.TryGetField("m_MaxWorkersPerCell", out FieldXml fieldConfig))
                compConfig.Fields.Add(new FieldXml { Name = "m_MaxWorkersPerCell", ValueFloat = item.Value });
        }
    }
}

ConfigurationXml config = ConfigToolXml.Config;

foreach (var item in PrefabSystem_AddPrefab_Patches.ProfitabilityDict)
{
    Plugin.Log($"DICT {item.Key} {item.Value}");
    PrefabXml prefabConfig = default(PrefabXml);
    if (!config.TryGetPrefab(item.Key, out prefabConfig))
        config.Prefabs.Add( new PrefabXml { Name = item.Key, Components = new List<ComponentXml>() });
    if (config.TryGetPrefab(item.Key, out prefabConfig))
    {
        ComponentXml compConfig = default(ComponentXml);
        if (!prefabConfig.TryGetComponent("CompanyPrefab", out compConfig))
            prefabConfig.Components.Add(new ComponentXml { Name = "CompanyPrefab", Fields = new List<FieldXml>() });
        if (prefabConfig.TryGetComponent("CompanyPrefab", out compConfig))
        {
            if (!compConfig.TryGetField("profitability", out FieldXml fieldConfig))
                compConfig.Fields.Add(new FieldXml { Name = "profitability", ValueFloat = item.Value });
        }
    }
}
*/
