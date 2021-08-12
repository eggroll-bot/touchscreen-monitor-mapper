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
		private readonly Dictionary<string, string> touchscreenIdToMonitorId = new( );

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
			string monitorAqs = DisplayMonitor.GetDeviceSelector( );
			DeviceInformationCollection monitors = await DeviceInformation.FindAllAsync( monitorAqs );

			if ( !touchscreens.Any( ) || !monitors.Any( ) )
			{
				MessageBox.Show( "No touchscreen or no monitor found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

				return;
			}

			ToolStripMenuItem touchscreensDropdownMenu = ( ToolStripMenuItem ) notifyIcon.ContextMenuStrip.Items[ 2 ];
			touchscreensDropdownMenu.DropDownItems.Clear( );

			for ( int i = 0; i < touchscreens.Count; i++ )
			{
				ToolStripMenuItem touchscreenDropdownMenu = new( "Touchscreen #" + ( i + 1 ) );
				touchscreensDropdownMenu.DropDownItems.Add( touchscreenDropdownMenu );

				foreach ( DeviceInformation monitor in monitors )
				{
					DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync( monitor.Id );
					string displayName = string.IsNullOrEmpty( displayMonitor.DisplayName ) ? monitor.Name : displayMonitor.DisplayName;
					ToolStripItem displayItem = touchscreenDropdownMenu.DropDownItems.Add( displayName );
					// TODO: If dictionary has no mapping from touchscreen to monitor, check the save file. If the save file has it, add the mapping to the dictionary and update the mapping in the registry.

					displayItem.Click += ( _, _ ) =>
					{
						touchscreenIdToMonitorId[ touchscreens[ i ].Id ] = monitor.Id;
						UpdateMapping( touchscreens[ i ].Id, monitor.Id );
						UpdateMappingSaveFile( );
					};
				}
			}
		}

		private void UpdateMapping( string touchscreenId, string monitorId )
		{
			// TODO: Update the mapping in the registry.
			throw new NotImplementedException( );
		}

		private void UpdateMappingSaveFile( )
		{
			// TODO: Update file that saves touchscreen to monitor mapping data.
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
