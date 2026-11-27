using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AE.Revit.Geometry.Domes;

namespace AE.Revit.Entry.Commands;

[Transaction(TransactionMode.Manual)]
public sealed class SmartDomeCommand : CommandBase
{
    private const string AppId="com.ahmedeztlou.aerevitaitools.smartdome";
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        if(document.IsFamilyDocument || document.IsReadOnly) { message="Open a writable project.";return Result.Cancelled; }
        var ui=application.ActiveUIDocument;
        var ids=ui.Selection.GetElementIds();
        var selected=ids.Count==1?document.GetElement(ids.First()) as DirectShape:null;
        if(selected?.ApplicationId!=AppId) selected=null;
        var state=selected is null?new UI.DomeInput():UI.DomeInput.Parse(selected.ApplicationDataId);
        var dialog=new UI.SmartDomeWindow(state);
        new System.Windows.Interop.WindowInteropHelper(dialog).Owner=application.MainWindowHandle;
        if(dialog.ShowDialog()!=true)return Result.Cancelled;
        var model=ProDomeGenerator.Generate(state.Settings);
        if(model.Members.Count>6000) {message="Reduce the resolution: maximum 6000 members per dome.";return Result.Cancelled;}
        XYZ origin;
        if(selected is null)
        {
            try { origin=ui.Selection.PickPoint("Pick dome centre on the active work plane"); }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException){return Result.Cancelled;}
            state.X=origin.X;state.Y=origin.Y;state.Z=origin.Z;
        }
        else origin=new XYZ(state.X,state.Y,state.Z);
        double f=UnitUtils.ConvertToInternalUnits(1,UnitTypeId.Meters);
        double radius=state.SectionMm/2000*f;
        XYZ Pt(DomeNode p)=>origin+new XYZ(p.X*f,p.Y*f,p.Z*f);
        var solids=new List<GeometryObject>();
        foreach(var m in model.Members)
        {
            var a=Pt(model.Nodes[m.A]);var b=Pt(model.Nodes[m.B]);var delta=b-a;
            double len=delta.GetLength();
            if(len<=application.Application.ShortCurveTolerance)throw new InvalidOperationException("Member shorter than Revit tolerance.");
            var axis=delta.Normalize();
            var u=axis.CrossProduct(Math.Abs(axis.Z)<0.9?XYZ.BasisZ:XYZ.BasisX).Normalize();
            var v=axis.CrossProduct(u).Normalize();
            var loop=new CurveLoop();
            // A square solid section is used for this conceptual representation.
            var pts=new[]{a+radius*(u+v),a+radius*(v-u),a-radius*(u+v),a+radius*(u-v)};
            for(int i=0;i<4;i++)loop.Append(Line.CreateBound(pts[i],pts[(i+1)%4]));
            solids.Add(GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>{loop},axis,len));
        }
        using(var tx=new Transaction(document,selected is null?"AE Create Dome":"AE Update Dome"))
        {
            tx.Start();
            var shape=selected??DirectShape.CreateElement(document,new ElementId(BuiltInCategory.OST_GenericModel));
            shape.ApplicationId=AppId;shape.ApplicationDataId=state.Serialize();
            shape.SetShape(solids);
            var comments=shape.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            if(comments!=null&&!comments.IsReadOnly)comments.Set(string.Format(CultureInfo.InvariantCulture,"AE {0}; {1} members; {2:F2} m total",state.Settings.Type,model.Members.Count,model.TotalLengthMetres));
            if(tx.Commit()!=TransactionStatus.Committed)throw new InvalidOperationException("Dome transaction did not commit.");
            ui.Selection.SetElementIds(new List<ElementId>{shape.Id});
        }
        TaskDialog.Show("AE Smart Dome",string.Format(CultureInfo.InvariantCulture,"{0} members | {1:F2} m total length.\nConceptual DirectShape only; not structural framing.",model.Members.Count,model.TotalLengthMetres));
        return Result.Succeeded;
    }
}
