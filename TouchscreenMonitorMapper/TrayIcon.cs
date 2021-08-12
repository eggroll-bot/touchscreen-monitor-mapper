using System;
using System.Drawing;
using System.Windows.Forms;

namespace TouchscreenMonitorMapper
{
	/// <summary>
	/// The tray icon class.
	/// </summary>
	internal class TrayIcon
	{
		private readonly NotifyIcon notifyIcon;

		/// <summary>
		/// Initializes a new instance of the <see cref="TrayIcon" /> class.
		/// </summary>
		internal TrayIcon( )
		{
			notifyIcon = new NotifyIcon( );
			notifyIcon.Icon = new Icon( AppDomain.CurrentDomain.BaseDirectory + "tray_icon.ico" );
			notifyIcon.Text = "Touchscreen Monitor Mapper";
			notifyIcon.ContextMenuStrip = new ContextMenuStrip( );
			notifyIcon.ContextMenuStrip.RenderMode = ToolStripRenderMode.System;
			PopulateTrayIconMenu( );
			notifyIcon.Visible = true;
		}

		/// <summary>
		/// Populates the tray icon menu.
		/// </summary>
		private void PopulateTrayIconMenu( )
		{
			ToolStripItemCollection trayItems = notifyIcon.ContextMenuStrip.Items;
			trayItems.Add( new ToolStripLabel( "Touchscreen Monitor Mapper" ) );
			trayItems.Add( new ToolStripSeparator( ) );
			trayItems.Add( new ToolStripLabel( "" ) );
			trayItems.Add( new ToolStripLabel( "" ) );
			trayItems.Add( new ToolStripSeparator( ) );
			trayItems.Add( "Exit", null, Exit );

			notifyIcon.MouseClick += ( object sender, MouseEventArgs eventArgs ) =>
			{
				if ( eventArgs.Button == MouseButtons.Right )
				{
					UpdateDevicesInUi( );
				}
			};
		}

		/// <summary>
		/// Updates touchscreens and monitors in the tray icon's dropdown menus.
		/// </summary>
		private void UpdateDevicesInUi( )
		{
			throw new NotImplementedException( );
		}

		/// <summary>
		/// Exits the application from the tray exit button.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void Exit( object sender, EventArgs e )
		{
			DialogResult toExit = MessageBox.Show( "Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question );

			if ( toExit == DialogResult.Yes )
			{
				notifyIcon.Dispose( );
				Application.Exit( );
			}
		}
	}
}
