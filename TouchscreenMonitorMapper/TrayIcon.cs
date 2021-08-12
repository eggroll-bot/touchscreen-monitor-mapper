using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Windows.Devices.Display;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;

namespace TouchscreenMonitorMapper
{
	/// <summary>
	/// The tray icon class.
	/// </summary>
	internal class TrayIcon
	{
		private const ushort hidUsagePageDigitizer = 0x0D;
		private const ushort hidUsageDigitizerTouchScreen = 0x04;
		private readonly NotifyIcon notifyIcon;
		private List<DeviceInformation> touchscreenList; // Use a list to ensure consistent ordering.
		private List<DeviceInformation> monitorList; // Use a list to ensure consistent ordering.

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
			trayItems.Add( new ToolStripMenuItem( "Touchscreens" ) );
			trayItems.Add( new ToolStripSeparator( ) );
			trayItems.Add( "Exit", null, Exit );

			notifyIcon.MouseClick += async ( object sender, MouseEventArgs eventArgs ) =>
			{
				if ( eventArgs.Button == MouseButtons.Right )
				{
					await UpdateDevicesInUi( );
				}
			};
		}

		/// <summary>
		/// Updates touchscreens and monitors in the tray icon's dropdown menus.
		/// </summary>
		private async Task UpdateDevicesInUi( )
		{
			string touchscreenAqs = HidDevice.GetDeviceSelector( hidUsagePageDigitizer, hidUsageDigitizerTouchScreen );
			DeviceInformationCollection touchscreens = await DeviceInformation.FindAllAsync( touchscreenAqs );
			touchscreenList = touchscreens.ToList( );
			string monitorAqs = DisplayMonitor.GetDeviceSelector( );
			DeviceInformationCollection monitors = await DeviceInformation.FindAllAsync( monitorAqs );
			monitorList = monitors.ToList( );

			if ( !touchscreenList.Any( ) || !monitorList.Any( ) )
			{
				MessageBox.Show( "No touchscreen or no monitor found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

				return;
			}

			ToolStripMenuItem touchscreensDropdownMenu = ( ToolStripMenuItem ) notifyIcon.ContextMenuStrip.Items[ 2 ];
			touchscreensDropdownMenu.DropDownItems.Clear( );

			for ( int i = 0; i < touchscreenList.Count; i++ )
			{
				ToolStripMenuItem touchscreenDropdownMenu = new ToolStripMenuItem( "Touchscreen #" + i + 1 );
				touchscreensDropdownMenu.DropDownItems.Add( touchscreenDropdownMenu );

				foreach ( DeviceInformation monitor in monitorList )
				{
					DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync( monitor.Id );
					string displayName = string.IsNullOrEmpty( displayMonitor.DisplayName ) ? monitor.Name : displayMonitor.DisplayName;
					touchscreenDropdownMenu.DropDownItems.Add( displayName );
				}
			}
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
