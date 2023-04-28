﻿#if IOS || MACCATALYST

using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SimpleToolkit.SimpleShell.Handlers.Platform
{
    public class SimpleNavigationController : UINavigationController, IUIGestureRecognizerDelegate
    {
        private CGPoint startingPoint = default;

        public event EventHandler PopGestureRecognized;


        protected virtual void OnPopGestureRecognized()
        {
            PopGestureRecognized?.Invoke(this, EventArgs.Empty);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            foreach (var subview in View.Subviews)
            {
                // Only resize a subview when it does not match the size of the parent
                if (subview.Bounds.Width != View.Bounds.Width || subview.Bounds.Height != View.Bounds.Height)
                {
                    subview.Frame = new CoreGraphics.CGRect(subview.Frame.X, subview.Frame.Y, View.Bounds.Width, View.Bounds.Height);
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InteractivePopGestureRecognizer.Delegate = this;
            InteractivePopGestureRecognizer.AddTarget((r) =>
            {
                if (r is not UIScreenEdgePanGestureRecognizer recognizer)
                    return;

                switch (recognizer.State)
                {
                    case UIGestureRecognizerState.Recognized:
                        var point = recognizer.LocationInView(ViewControllers.FirstOrDefault()?.View);

                        if (point.X >= startingPoint.X)
                            OnPopGestureRecognized();
                        break;
                    case UIGestureRecognizerState.Began:
                        startingPoint = recognizer.LocationInView(ViewControllers.FirstOrDefault()?.View);
                        break;
                }
            });
        }

        [Export("gestureRecognizerShouldBegin:")]
        public bool ShouldBegin(UIGestureRecognizer recognizer)
        {
            return true;
        }
    }
}

#endif