using System;
using System.Threading;
using System.Windows.Forms;

namespace TouchscreenMonitorMapper
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		internal static void Main( )
		{
			using Mutex appMutex = new( false, "Global\\{f4cd6773-07b7-4e68-ad16-4a494c55b3c3}" + Environment.UserName );

			if ( !appMutex.WaitOne( 0, false ) )
			{
				MessageBox.Show( "Application is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

				return;
			}

			Application.EnableVisualStyles( );
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new App( ) );
		}
	}
}
