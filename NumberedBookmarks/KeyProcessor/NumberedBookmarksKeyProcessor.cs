//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.Text.Editor;
//using System.Windows.Input;

//namespace NumberedBookmarks
//{
//    class NumberedBookmarksKeyProcessor : KeyProcessor
//    {
//        private Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView;

//        public override bool IsInterestedInHandledEvents
//        {
//            get
//            {
//                return true;
//            }
//        }

//        class cmd : ICommand
//        {
//            #region ICommand Members

//            public bool CanExecute(object parameter)
//            {
//                return true;
//            }

//            public event EventHandler CanExecuteChanged;

//            public void Execute(object parameter)
//            {
//                System.Diagnostics.Trace.WriteLine("cmd execute");
//            }

//            #endregion
//        }

//        public NumberedBookmarksKeyProcessor(Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView)
//        {
//            this.wpfTextView = wpfTextView;
//            wpfTextView.Closed += new EventHandler(wpfTextView_Closed);
//            Keyboard.AddPreviewKeyDownHandler(wpfTextView.VisualElement, KeyEventHandler);
//            //System.Windows.Input.AccessKeyManager.AddAccessKeyPressedHandler(wpfTextView.VisualElement, AccessKeyPressedEventHandler);

//            System.Windows.Input.InputManager.Current.PreProcessInput += new PreProcessInputEventHandler(Current_PreProcessInput);
//            var cmdBinding = new CommandBinding(new cmd());
//            //
//            ////this.wpfTextView.VisualElement.CommandBindings.Add(new CommandBinding());
//            //KeyBinding k = new KeyBinding(cmdBinding.Command, new KeyGesture(Key.D8, ModifierKeys.Control | ModifierKeys.Shift));
//            //this.wpfTextView.VisualElement.CommandBindings.Add(cmdBinding);
//            //this.wpfTextView.VisualElement.InputBindings.Add(k);
//        }

//        void Current_PreProcessInput(object sender, PreProcessInputEventArgs e)
//        {
//            if (e.StagingItem.Input.Device == null)
//                return;
//            var device = e.StagingItem.Input.Device as KeyboardDevice;
//            if (device != null)
//            {
//                if (device.FocusedElement == this.wpfTextView)
//                {
//                    System.Diagnostics.Trace.WriteLine("Current_PreProcessInput)");
//                    if (device.IsKeyDown(Key.D8))
//                    {
//                        System.Diagnostics.Trace.WriteLine("D8 is down)");
//                    }
//                    return;
//                }
//                var s = "keyboard";
                
//                System.Diagnostics.Trace.WriteLine(string.Format("Current_PreProcessInput({0}, {1})", s, device.FocusedElement ==null  ? "" : device.FocusedElement.ToString()));            
//            }
//        }

//        void wpfTextView_Closed(object sender, EventArgs e)
//        {
            
//            Keyboard.RemovePreviewKeyDownHandler(wpfTextView.VisualElement, KeyEventHandler);
//        }

//        void AccessKeyPressedEventHandler(object sender, AccessKeyPressedEventArgs e)
//        {
//            System.Diagnostics.Trace.WriteLine(string.Format("AccessKeyPressedEventHandler({0})", e.Key));
//        }

//        void KeyEventHandler(object sender, KeyEventArgs e)
//        {
//            if (this.wpfTextView.HasAggregateFocus)
//            {
//                System.Diagnostics.Trace.WriteLine(string.Format("KeyEventHandler ({0})", e.Key));
//            }
//        }

//        public override void PreviewKeyDown(KeyEventArgs args)
//        {
//            //System.Diagnostics.Trace.WriteLine(string.Format("PreviewKeyDown {0}", args.Key));
//            base.PreviewKeyDown(args);
//        }

//        public override void KeyDown(System.Windows.Input.KeyEventArgs args)
//        {
//            //System.Diagnostics.Trace.WriteLine(string.Format("KeyDown {0}", args.Key));
//            var modifiers = (System.Windows.Input.ModifierKeys.Shift | System.Windows.Input.ModifierKeys.Control);
//            if ((Keyboard.Modifiers  & modifiers) == modifiers)
//            {
//                int keyNumber = (int)args.Key - (int)System.Windows.Input.Key.D0;
//                if (keyNumber >= 0 && keyNumber <= 9)
//                {
//                    var manager = NumberedBookmarksManager.GetBookmarkManager(this.wpfTextView);
//                    manager.ToogleBookmark(this.wpfTextView.Caret.Position, keyNumber);
//                    args.Handled = true;
//                }
//            }
//            else if ((Keyboard.Modifiers &  System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
//            {
//                int keyNumber = (int)args.Key - (int)System.Windows.Input.Key.D0;
//                if (keyNumber >= 0 && keyNumber <= 9)
//                {
//                    var manager = NumberedBookmarksManager.GetBookmarkManager(this.wpfTextView);
//                    manager.GotoBookmark(this.wpfTextView, keyNumber);
//                    args.Handled = true;

//                }
//            }
//            else
//                base.KeyDown(args);
//        }
//    }
//}
