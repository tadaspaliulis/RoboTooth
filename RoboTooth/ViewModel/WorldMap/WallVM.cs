using Extensions;
using RoboTooth.Model.Mapping;
using RoboTooth.ViewModel.Drawing;
using System;
using System.ComponentModel;

namespace RoboTooth.ViewModel.WorldMap
{
    public class WallVM : DrawableObservable
    {
        public WallVM(Wall wall)
        {
            if (wall == null)
                throw new ArgumentNullException(nameof(wall));

            CalculateLines(wall);

            wall.WallUpdated += HandleWallUpdated;
        }

        public void HandleWallUpdated(object sender, EventArgs e)
        {
            if (!(sender is Wall wall))
                throw new ArgumentException(nameof(sender));

            CalculateLines(wall);
        }

        public override void HandleViewPortChange(object sender, PropertyChangedEventArgs e)
        {
            //Don't need to do anything here for now
        }

        public override void SetViewPortSettings(ViewPortSettings viewPortSettings)
        {
            //Make sure the lines get subscribed to ViewPortSettings events
            base.SetViewPortSettings(viewPortSettings);
            MainLine.SetViewPortSettings(viewPortSettings);
            ShadowLineInner.SetViewPortSettings(viewPortSettings);
            ShadowLineOuter.SetViewPortSettings(viewPortSettings);
        }

        private void CalculateLines(Wall wall)
        {
            var leftFacing = wall.FaceNormal.PerpendicularCounterClockWise();
            var rightFacing = -leftFacing;

            var startPoint = leftFacing * wall.LengthLeft + wall.Position;
            var endPoint = rightFacing * wall.LengthRight + wall.Position;

            MainLine = new Line(startPoint, endPoint);

            var innerShadowOffset = -wall.FaceNormal * LINE_GAP;
            var startPointInnerShadow = leftFacing * INNER_SHADOW_SCALING *
                wall.LengthLeft + wall.Position + innerShadowOffset;
            var endPointInnerShadow = rightFacing * INNER_SHADOW_SCALING *
                wall.LengthRight + wall.Position + innerShadowOffset;

            ShadowLineInner = new Line(startPointInnerShadow, endPointInnerShadow);

            var outerShadowOffset = -wall.FaceNormal * LINE_GAP * 2;
            var startPointOuterShadow = leftFacing * OUTER_SHADOW_SCALING *
                wall.LengthLeft + wall.Position + outerShadowOffset;
            var endPointOuterShadow = rightFacing * OUTER_SHADOW_SCALING *
                wall.LengthRight + wall.Position + outerShadowOffset;

            ShadowLineOuter = new Line(startPointOuterShadow, endPointOuterShadow);
        }

        private const float LINE_GAP = 10.0f;
        private const float INNER_SHADOW_SCALING = 0.8f;
        private const float OUTER_SHADOW_SCALING = 0.64f;

        #region Observable Properties

        private Line _mainLine;

        public Line MainLine
        {
            get { return _mainLine; }
            set
            {
                _mainLine = value;
                NotifyPropertyChanged();
            }
        }

        private Line _shadowLineInner;

        public Line ShadowLineInner
        {
            get { return _shadowLineInner; }
            set
            {
                _shadowLineInner = value;
                NotifyPropertyChanged();
            }
        }

        private Line _shadowLineOuter;

        public Line ShadowLineOuter
        {
            get { return _shadowLineOuter; }
            set
            {
                _shadowLineOuter = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
