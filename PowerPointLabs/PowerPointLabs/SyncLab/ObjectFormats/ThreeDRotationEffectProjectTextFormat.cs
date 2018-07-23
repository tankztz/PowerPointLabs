﻿using System;
using System.CodeDom;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows;
using Microsoft.Office.Core;
using PowerPointLabs.ActionFramework.Common.Log;
using PowerPointLabs.EffectsLab;
using PowerPointLabs.Utils;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;
using Shapes = Microsoft.Office.Interop.PowerPoint.Shapes;
using ThreeDFormat = Microsoft.Office.Interop.PowerPoint.ThreeDFormat;

namespace PowerPointLabs.SyncLab.ObjectFormats
{
    class ThreeDRotationEffectProjectTextFormat : Format
    {
        private static readonly float TOLERANCE = Single.Epsilon;

        public override bool CanCopy(Shape formatShape)
        {
            ThreeDFormat threeD = formatShape.ThreeD;

            // equality check for floating point numbers
            return Math.Abs(threeD.RotationX) > TOLERANCE
                   || Math.Abs(threeD.RotationY) > TOLERANCE
                   || Math.Abs(threeD.RotationZ) > TOLERANCE
                   || Math.Abs(threeD.FieldOfView) > TOLERANCE
                   || threeD.Perspective == MsoTriState.msoTrue;
        }

        public override void SyncFormat(Shape formatShape, Shape newShape)
        {
            if (!Sync(formatShape, newShape))
            {
                Logger.Log(newShape.Type + " unable to sync " + this.GetType().Name);
            }
        }

        public override Bitmap DisplayImage(Shape formatShape)
        {
            // ProjectText specifies whether text on the specified ThreeDFormat object rotates with the object. 
            // true means rotares with object, implies "No" for keep text flat
            return SyncFormatUtil.GetTextDisplay(
                ((formatShape.ThreeD.ProjectText==MsoTriState.msoTrue)?"No":"Yes"),
                SyncFormatConstants.DisplayImageFont,
                SyncFormatConstants.DisplayImageSize);
        }

        private static bool Sync(Shape formatShape, Shape newShape)
        {
            ThreeDFormat source = formatShape.ThreeD;
            ThreeDFormat dest = newShape.ThreeD;
            
            try
            {
                dest.ProjectText = source.ProjectText;
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }
    }
}