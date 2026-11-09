using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace AE.Revit.Entry;
internal sealed class OwnedFacade { internal OwnedFacade(DirectShape shape,FacadeMetadata metadata){Shape=shape;Metadata=metadata;} internal DirectShape Shape{get;} internal FacadeMetadata Metadata{get;} }
internal static class FacadeDiscovery
{
    internal static IReadOnlyList<OwnedFacade> Find(Document document) => new FilteredElementCollector(document).OfClass(typeof(DirectShape)).Cast<DirectShape>().Select(shape=>new {shape,metadata=FacadeStorage.Read(shape)}).Where(x=>x.metadata!=null&&!string.IsNullOrWhiteSpace(x.metadata.HostUniqueId)&&!string.IsNullOrWhiteSpace(x.metadata.ConfigurationId)).Select(x=>new OwnedFacade(x.shape,x.metadata!)).ToArray();
    internal static OwnedFacade? FindByConfiguration(Document document,string configurationId) => Find(document).FirstOrDefault(x=>string.Equals(x.Metadata.ConfigurationId,configurationId,StringComparison.Ordinal));
}
