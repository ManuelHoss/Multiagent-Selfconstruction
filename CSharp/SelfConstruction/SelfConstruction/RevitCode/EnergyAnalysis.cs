using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;

namespace SelfConstruction.RevitCode
{
    class EnergyAnalysis
    {
        private EnergyAnalysis() { }

        public static EnergyAnalysis Instance { get; } = new EnergyAnalysis();

        public void CalculateAndDisplayVolumeAndArea(Document doc)
        {
            CreateRooms(doc);
            CreateSchedule(doc);

            AreaVolumeSettings settings = AreaVolumeSettings.GetAreaVolumeSettings(doc);
            settings.ComputeVolumes = true;
        }

        public double[] GetAreaAndVolume(Document doc)
        {
            FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));
            var sectionData = ((ViewSchedule)col.FirstElement()).GetTableData().GetSectionData(SectionType.Body);

            double area = 0;
            double volume = 0;
            for (int j = 0; j < sectionData.LastRowNumber; j++)
            {
                double tempArea;
                Double.TryParse(Regex.Match(sectionData.GetCellText(j, 0), @"\d+\.\d+").Value, out tempArea);
                double tempVolume;
                Double.TryParse(Regex.Match(sectionData.GetCellText(j, 1), @"\d+\.\d+").Value, out tempVolume);
                area += tempArea;
                volume += tempVolume;
            }
            return new []{area, volume};
        }
        private static void CreateRooms(Document doc)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan));

            //This list is needed, because otherwise there is an error, of multiple roomdeclarations of the same room
            List<string> accessedLevels = new List<string>();

            foreach (var element in viewCollector)
            {
                if (element.Name.IndexOf("Level", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                    element.Name.IndexOf("Ebene", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var viewPlan = (ViewPlan) element;
                    if (!accessedLevels.Contains(viewPlan.ViewName))
                    {
                        accessedLevels.Add(viewPlan.ViewName);
                        doc.Create.NewRooms2(viewPlan.GenLevel);
                    }
                }
            }
        }
        private void CreateSchedule(Document doc)
        {
            ViewSchedule vs = ViewSchedule.CreateSchedule(doc, new ElementId(BuiltInCategory.OST_Rooms));

            doc.Regenerate();

            AddRegularFieldToSchedule(vs, new ElementId(BuiltInParameter.ROOM_AREA));
            AddRegularFieldToSchedule(vs, new ElementId(BuiltInParameter.ROOM_VOLUME));
            AddRegularFieldToSchedule(vs, new ElementId(BuiltInParameter.ROOM_LEVEL_ID));
        }

        public static void AddRegularFieldToSchedule(ViewSchedule schedule, ElementId paramId)
        {
            ScheduleDefinition definition = schedule.Definition;

            SchedulableField schedulableField =
                definition.GetSchedulableFields().FirstOrDefault<SchedulableField>(sf => sf.ParameterId == paramId);

            if (schedulableField != null)
            {
                definition.AddField(schedulableField);
            }
        }

        [Obsolete("SpacesEnergyAnalysis is deprecated, because spaces is not used anymore. Could be deleted")]
        public void SpacesEnergyAnalysis(Document doc)
        {
            EnergyAnalysisDetailModelOptions options = new EnergyAnalysisDetailModelOptions
            {
                Tier = EnergyAnalysisDetailModelTier.NotComputed,
                EnergyModelType = EnergyModelType.SpatialElement
            };

            EnergyAnalysisDetailModel analysisDetailModel = EnergyAnalysisDetailModel.Create(doc, options);

            IList<EnergyAnalysisSpace> spaces = analysisDetailModel.GetAnalyticalSpaces();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Spaces: " + spaces.Count);
            foreach (EnergyAnalysisSpace space in spaces)
            {
                builder.AppendLine(space.Name + " InnVolume " + space.InnerVolume);
            }
            TaskDialog.Show("EAM", builder.ToString());
        }
    }
}
