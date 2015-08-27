using System;

namespace DbUtils.UI
{
	public class TabLabel : Gtk.HBox
	{

		public TabLabel (String labelText) :
		base()
		{
			this.LabelText = labelText;
			this.Init ();
		}

		private void Init() {
			this.Spacing = 5;

			this.CanFocus = true;

			// icon
			var icon = Gtk.Image.NewFromIconName(Gtk.Stock.File, Gtk.IconSize.Menu);
			this.PackStart(icon, true, true, 0);

			// label
			var Label = new global::Gtk.Label(this.LabelText);
			this.PackStart (Label, true, true, 0);

			// Close button
			var closeBtn = new global::Gtk.Button();
			closeBtn.Relief = Gtk.ReliefStyle.None;
			closeBtn.FocusOnClick = false;
			closeBtn.Add(Gtk.Image.NewFromIconName(Gtk.Stock.Close, Gtk.IconSize.Menu));
			closeBtn.BorderWidth = 0;
			closeBtn.Clicked += (sender, e) => OnCloseClicked();

			this.PackStart(closeBtn, false, false, 0);

			this.ShowAll();

		}

		public event EventHandler CloseClicked; 

		/// <summary>
		/// Raises the close clicked event.
		/// </summary>
		protected virtual void OnCloseClicked() {
			if (CloseClicked != null) {
				CloseClicked (this, EventArgs.Empty);
			}
		}

		public String LabelText {
			get;
			private set;
		}
	}
}

