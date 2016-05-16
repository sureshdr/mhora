// SplashScreen.cs: Contributed by Peter Foreman
// A splash screen class
#region Copyright © 2002-2004 The Genghis Group
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from the
 * use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not claim
 * that you wrote the original software. If you use this software in a product,
 * an acknowledgment in the product documentation is required, as shown here:
 *
 * Portions Copyright © 2002-2004 The Genghis Group (http://www.genghisgroup.com/).
 *
 * 2. No substantial portion of the source code of this library may be redistributed
 * without the express written permission of the copyright holders, where
 * "substantial" is defined as enough code to be recognizably from this library.
*/
#endregion
#region Features
/*
 * -Makes a form into a splash screen
 * -Designed with a minimal interface to increase ease of use
 * -Multithreaded to ensure message queue is pumped whilst main thread is busy
*/
#endregion
#region Limitations
/*
 * 
*/
#endregion
#region History
/*
 *
*/
#endregion

using System;
using System.Windows.Forms;

namespace Genghis.Windows.Forms
{
    [Flags]
    public enum SplashScreenStyles
    {
        None = 0,
        /// <summary>
        /// Normally the splash is automatically set to be center of screen and without a title bar
        /// overriding whatever styles normally apply.  
        /// Set this to avoid those styles being applied.
        /// </summary>
        DontSetFormStyles = 1,
        /// <summary>
        /// Makes the created splash screen top most - not recommended as it can be irritating for users.
        /// See Form.TopMost for more details.
        /// </summary>
        TopMost = 2,
        /// <summary>
        /// Normally the splash is created with a FixedSingle border style.  
        /// Set this to avoid that style being applied.
        /// </summary>
        DontSetBorderStyle = 4
    }
    
    /// <summary>
    /// Splash Screen class
    /// </summary>
    public class SplashScreen
    {
        private System.Threading.Thread thread = null;
        private SplashScreenWindow window = null;
        private bool openWindowAllowed = true;
        private SplashScreenStyles styles = SplashScreenStyles.None;
        private Type formType = null;

        /// <summary>
        /// Create a splash screen based on a Form.  
        /// form must be System.Windows.Forms.Form or derived from it.  
        /// Typical usage: <c>SplashScreen ss = new SplashScreen( typeof( MyForm ) );</c>.
        /// </summary>
        public SplashScreen( Type formType )
        {
            this.formType = formType;
            Initialize();
        }    
    
        /// <summary>
        /// Create a splash screen based on a Form, with non standard styles.  
        /// form must be System.Windows.Forms.Form or derived from it.  
        /// Typical usage: <c>SplashScreen ss = new SplashScreen( typeof( MyForm ), SplashScreenStyles.TopMost ); );</c>.
        /// </summary>
        public SplashScreen( Type formType, SplashScreenStyles styles )
        {
            this.formType = formType;
            this.styles = styles;
            Initialize();
        }

        private void Initialize()
        {
            if( !formType.IsSubclassOf( typeof( Form ) ) )
                throw new ArgumentException( "The type passed in must be a Form, or subclass.", "formType" );
            CreateSplashScreenThread();
        }

        /// <summary>
        /// Closes the splash screen immediately.  
        /// Activates the form specified by main (may be null).
        /// </summary>
        // Thread safe
        public void Close( Form main )
        {
            Close( main, 0 );
        }

        /// <summary>
        /// Closes the splash screen after a specific time period (specified in milliseconds).  
        /// Activates the form specified by main (may be null).
        /// </summary>
        // Thread safe
        public void Close( Form main, int milliseconds )
        {
            lock( this )
            {
                // It could (in theory) be possible to call Close() before the window has been created
                // on the thread - this flag prevents that happening
                openWindowAllowed = false;
                if( window != null )
                    window.Close( main, milliseconds );
            }
        }

        // Only call from ctor
        private void CreateSplashScreenThread()
        {            
            thread = new System.Threading.Thread( new System.Threading.ThreadStart( ThreadFunction ) );
            thread.Name = "Splash Screen";
            thread.ApartmentState = System.Threading.ApartmentState.STA;
            
            thread.Start();
            System.Threading.Thread.Sleep( 0 );
        }

        // Only call when creating new thread in CreateSplashScreenThread
        private void ThreadFunction()
        {
            lock( this )
            {
                if( openWindowAllowed )
                    window = new SplashScreenWindow( formType, styles );    
            }

            if( window != null )
                window.EnterMessagePump();
        }

        /// <summary>
        /// Internal class whose lifetime (between ctor and Close) determines how long the 
        /// splash screen remains visible
        /// </summary>
        private class SplashScreenWindow
        {
            private Form form = null;
            private Form main = null;

            private delegate void CloseDelegate();

            public SplashScreenWindow( Type formType, SplashScreenStyles styles )
            {
                CreateForm( formType );
                
                form.ShowInTaskbar = false;
                form.TopLevel = true;

                if( (styles & SplashScreenStyles.TopMost) != 0 )
                    form.TopMost = true; // Yuk!  Do you really have to

                // If bit not set then set border
                if( (styles & SplashScreenStyles.DontSetBorderStyle) == 0 )
                    form.FormBorderStyle = FormBorderStyle.FixedSingle;

                // If bit not set then SetStyles
                if( (styles & SplashScreenStyles.DontSetFormStyles) == 0 )
                    SetStyles();
            }

            public void EnterMessagePump()
            {
                Application.Run( form );
            }

            //Call from ctor only
            private void CreateForm( Type formType )
            {
                // Get default constructor
                System.Reflection.ConstructorInfo constructor = formType.GetConstructor( Type.EmptyTypes );
                form = constructor.Invoke( null ) as Form;
            }

            //Call from ctor only
            private void SetStyles()
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.Name = "";
                form.Text = "";
                form.ControlBox = false;
            }

            //Thread safe
            public void Close( Form main, int milliseconds )
            {
                this.main = main;

                if( milliseconds <= 0 )
                    CloseNow();
                else
                    SetupCloseTimer( milliseconds );
            }

            private void SetupCloseTimer( int milliseconds )
            {
                lock( this )
                {
                    Timer timer = new Timer();
                    timer.Interval = milliseconds;
                    timer.Tick += new EventHandler( ElapsedEventHandler );
                    timer.Start();
                }
            }

            private void ElapsedEventHandler( object sender, EventArgs e )
            {
                Timer timer = sender as Timer;
                timer.Stop();
                CloseNow();
            }

            private void CloseNow()
            {
                lock( this )
                {
                    if( main != null )
                        main.Activate();
                    if( form != null )
                        form.Invoke( new CloseDelegate( form.Close ) );
                    form = null;
                }
            }
        }//class SplashScreenWindow
    
    }//class SplashScreen
}
