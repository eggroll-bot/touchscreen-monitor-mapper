using System.Windows.Forms;

namespace TouchscreenMonitorMapper
{
	/// <summary>
	/// The application class.
	/// </summary>
	internal class App : ApplicationContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="App" /> class.
		/// </summary>
		internal App( )
		{
			new TrayIcon( );
		}
	}
}