﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Biomorpher.IGA
{
    /// <summary>
    /// Branch containing a list of biomorpher generations
    /// </summary>
    public class BioBranch
    {
        /// <summary>
        /// Source branchID where growth began
        /// </summary>
        public int ParentBranchIndex { get; set; }
        
        /// <summary>
        /// Source twig index where growth began
        /// </summary>
        public int ParentTwigIndex { get; set; }

        /// <summary>
        /// Location of starting y coordinate on the history canvas
        /// </summary>
        public int StartY { get; set; }

        /// <summary>
        /// List of populations in this branch
        /// </summary>
        public List<Population> Twigs { get; set; }

        /// <summary>
        /// The BSpline curve drawn back to the parent
        /// </summary>
        public Path OriginCurve { get; set; }

        /// <summary>
        /// Contructor for a BioBranch
        /// </summary>
        /// <param name="parentBranchIndex">Every BioBranch has a parent branch. Use -1 for initial branch</param>
        /// <param name="parentTwigIndex">Every new branch can grow from a twig. Store index this twig here</param>
        /// <param name="startY">Location of starting y coordinate on the history canvas</param>
        public BioBranch(int parentBranchIndex, int parentTwigIndex, int startY)
        {
            Twigs = new List<Population>();
            this.ParentBranchIndex = parentBranchIndex;
            this.ParentTwigIndex = parentTwigIndex;
            StartY = startY; // used for the history bit
        }

        /// <summary>
        /// Adds a new population 'twig'
        /// </summary>
        /// <param name="pop">Population that will be copied and stored</param>
        public void AddTwig(Population pop)
        {
            Twigs.Add(new Population(pop)); // copy of the current pop
        }


        /// <summary>
        /// Draws a BSpline curve from parent branch to new branch
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="allBranches"></param>
        public void DrawOriginCurve(Canvas canvas, List<BioBranch> allBranches)
        {
            OriginCurve = new Path();

            System.Windows.Point outNode = allBranches[ParentBranchIndex].Twigs[ParentTwigIndex].HistoryNodeOUT;
            System.Windows.Point inNode = this.Twigs[0].HistoryNodeIN;

            int offset = (int)((inNode.X - outNode.X)/2);

            // Add a random factor to separate lines.
            int randomY = Friends.GetRandomInt(0, 32);

            System.Windows.Point P1 = new System.Windows.Point(outNode.X, outNode.Y + randomY);
            System.Windows.Point P2 = new System.Windows.Point(outNode.X + offset, outNode.Y + randomY);
            System.Windows.Point P3 = new System.Windows.Point(inNode.X - offset, inNode.Y + randomY);
            System.Windows.Point P4 = new System.Windows.Point(inNode.X, inNode.Y + randomY);

            OriginCurve.Data = Friends.MakeBezierGeometry(P1, P2, P3, P4);
            OriginCurve.Stroke = Brushes.LightGray;
            OriginCurve.StrokeThickness = 0.6;
            //Canvas.SetLeft(OriginCurve, 0);
            //Canvas.SetTop(OriginCurve, 0);
            Canvas.SetZIndex(OriginCurve, -1);
            canvas.Children.Add(OriginCurve);


            //Add outline circle
            double s = 6;
            System.Windows.Shapes.Ellipse nodule = new System.Windows.Shapes.Ellipse();
            nodule.Height = s;
            nodule.Width = s;
            nodule.Fill = Brushes.LightGray;
            System.Windows.Shapes.Ellipse nodule2 = new System.Windows.Shapes.Ellipse();
            nodule2.Height = s;
            nodule2.Width = s;
            nodule2.Fill = Brushes.LightGray;

            Canvas.SetLeft(nodule, P1.X-s/2);
            Canvas.SetTop(nodule, P1.Y-s/2);
            canvas.Children.Add(nodule);

            Canvas.SetLeft(nodule2, P4.X-s/2);
            Canvas.SetTop(nodule2, P4.Y-s/2);
            canvas.Children.Add(nodule2);

            
        }

    }
}
