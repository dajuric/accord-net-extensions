using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Windows.Forms;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using RangeF = AForge.Range;

namespace ObjectAnnotater.SampleGeneration
{
    public partial class SamplePreparation : Form
    {
        Database database;

        public SamplePreparation(Database database = null)
        { 
            InitializeComponent();
            this.database = database;
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (database == null)
                return;

            var newDb = generateSamples
                (
                   database: this.database,
                   //1) inflate
                   inflateFactor: (float)this.nInflateFactor.Value,
                   //2) random
                   locationRangeX: new RangeF((float)nLocMinX.Value, (float)nLocMaxX.Value),
                   locationRangeY: new RangeF((float)nLocMinY.Value, (float)nLocMaxY.Value),

                   scaleRangeWidth: new RangeF((float)nScaleMinWidth.Value, (float)nScaleMaxWidth.Value),
                   scaleRangeHeight: new RangeF((float)nScaleMinHeight.Value, (float)nScaleMaxHeight.Value),

                   nSamples: (int)nSanples.Value,
                   //3) unify scales
                   widthHeightRatio: (float)nWidhtHeightRatio.Value
                );

            using (var diag = new SaveFileDialog())
            {
                diag.Filter = "(*.xml)|*.xml";
                diag.OverwritePrompt = true;

                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    newDb.Save(diag.FileName);
                }
            }

            MessageBox.Show("Saved!", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static Database generateSamples(Database database, 
                                                //1) inflate
                                                float inflateFactor,
                                                //2) random
                                                RangeF locationRangeX, RangeF locationRangeY, RangeF scaleRangeWidth, RangeF scaleRangeHeight, int nSamples,
                                                //3) unify scales
                                                float widthHeightRatio)
        {
            return database.ProcessSamples(widthHeightRatio,
                                           new Pair<RangeF>(locationRangeX, locationRangeY),
                                           new Pair<RangeF>(scaleRangeWidth, scaleRangeHeight),
                                           nSamples,
                                           inflateFactor);
        }

        private void nScaleWidth_ValueChanged(object sender, EventArgs e)
        {
            this.nScaleMinWidth.Value = Math.Min(this.nScaleMinWidth.Value, this.nScaleMaxWidth.Value);
            this.nScaleMaxWidth.Value = Math.Max(this.nScaleMinWidth.Value, this.nScaleMaxWidth.Value);
        }

        private void nScaleHeight_ValueChanged(object sender, EventArgs e)
        {
            this.nScaleMinHeight.Value = Math.Min(this.nScaleMinHeight.Value, this.nScaleMaxHeight.Value);
            this.nScaleMaxHeight.Value = Math.Max(this.nScaleMinHeight.Value, this.nScaleMaxHeight.Value);
        }

        private void nLocX_ValueChanged(object sender, EventArgs e)
        {
            this.nLocMinX.Value = Math.Min(this.nLocMinX.Value, this.nLocMaxX.Value);
            this.nLocMaxX.Value = Math.Max(this.nLocMinX.Value, this.nLocMaxX.Value);
        }

        private void nLocY_ValueChanged(object sender, EventArgs e)
        {
            this.nLocMinY.Value = Math.Min(this.nLocMinY.Value, this.nLocMaxY.Value);
            this.nLocMaxY.Value = Math.Max(this.nLocMinY.Value, this.nLocMaxY.Value);
        }
    }
}
