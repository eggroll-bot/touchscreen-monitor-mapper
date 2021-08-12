using System;
using System.Windows.Forms;

using Microsoft.Win32;

namespace TouchscreenMonitorMapper
{
	/// <summary>
	/// A class with various helper methods needed for the application.
	/// </summary>
	public static class TouchscreenMonitorHelper
	{
		private const string touchscreenSettingsRegistryKeyPath = @"SOFTWARE\Microsoft\Wisp\Pen\Digimon";

		/// <summary>
		/// Updates the mapping between a touchscreen and monitor in the registry.
		/// </summary>
		/// <param name="touchscreenId">The device ID of the touchscreen.</param>
		/// <param name="monitorId">The device ID of the monitor.</param>
		public static void UpdateMapping( string touchscreenId, string monitorId )
		{
			try
			{
				RegistryKey touchscreenSettingsRegistryKey = Registry.LocalMachine.OpenSubKey( touchscreenSettingsRegistryKeyPath, true );

				if ( touchscreenSettingsRegistryKey == null )
				{
					MessageBox.Show( "Could not write to registry. Registry key not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

					return;
				}

				string touchscreenRegistryValueToSet = null;
				string[ ] touchscreenRegistryValues = touchscreenSettingsRegistryKey.GetValueNames( );

				foreach ( string touchscreenRegistryValue in touchscreenRegistryValues )
				{
					if ( touchscreenRegistryValue.Contains( touchscreenId ) )
					{
						touchscreenRegistryValueToSet = touchscreenRegistryValue;
						break;
					}
				}

				if ( touchscreenRegistryValueToSet == null )
				{
					touchscreenSettingsRegistryKey.Close( );
					MessageBox.Show( "Could not write to registry. Touchscreen does not exist in the registry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

					return;
				}

				touchscreenSettingsRegistryKey.SetValue( touchscreenRegistryValueToSet, monitorId );
				touchscreenSettingsRegistryKey.Close( );
				RestartWisptis( );
			} catch
			{
				MessageBox.Show( "Could not write to registry. Insufficient permissions. Try running the program as administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

				return;
			}
		}

		/// <summary>
		/// Restarts wisptis.exe to allow the registry changes to take effect.
		/// </summary>
		private static void RestartWisptis( )
		{
			// TODO: Implement.
			throw new NotImplementedException( );
		}
	}
}
