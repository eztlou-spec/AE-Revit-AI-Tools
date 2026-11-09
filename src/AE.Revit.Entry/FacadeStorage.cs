using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace AE.Revit.Entry;
internal sealed class FacadeMetadata { internal FacadeMetadata(string host,string config){HostUniqueId=host;ConfigurationId=config;} internal string HostUniqueId{get;} internal string ConfigurationId{get;} }
internal static class FacadeStorage
{
    private static readonly Guid SchemaId=new("a92cae66-ea60-4b5f-ae9b-c15f6de4b929");
    private static Schema GetSchema(){ var schema=Schema.Lookup(SchemaId); if(schema!=null)return schema; var b=new SchemaBuilder(SchemaId); b.SetSchemaName("AEFacadeMetadata"); b.SetReadAccessLevel(AccessLevel.Public); b.SetWriteAccessLevel(AccessLevel.Vendor); b.SetVendorId("AETL"); b.AddSimpleField("HostUniqueId",typeof(string)); b.AddSimpleField("ConfigurationId",typeof(string)); return b.Finish(); }
    internal static void Write(Element element,FacadeMetadata data){var schema=GetSchema();var entity=new Entity(schema);entity.Set("HostUniqueId",data.HostUniqueId);entity.Set("ConfigurationId",data.ConfigurationId);element.SetEntity(entity);}
    internal static FacadeMetadata? Read(Element element){var schema=Schema.Lookup(SchemaId);if(schema==null)return null;var entity=element.GetEntity(schema);return !entity.IsValid()?null:new FacadeMetadata(entity.Get<string>("HostUniqueId"),entity.Get<string>("ConfigurationId"));}
}
