// AsyncForm.cs
using System.Windows.Forms;

namespace ModernCECTray.UI
{
    public class AsyncForm2 : Form
    {
        protected void SetControlEnabled(Control control, bool enabled)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(() => control.Enabled = enabled));
            }
            else
            {
                control.Enabled = enabled;
            }
        }

        protected void SetControlText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(() => control.Text = text));
            }
            else
            {
                control.Text = text;
            }
        }

        protected void SetCheckboxChecked(CheckBox checkbox, bool check)
        {
            if (checkbox.InvokeRequired)
            {
                checkbox.Invoke(new MethodInvoker(() => checkbox.Checked = check));
            }
            else
            {
                checkbox.Checked = check;
            }
        }

        protected void SetComboBoxItems(ComboBox comboBox, int selectedIndex, object[] items)
        {
            if (comboBox.InvokeRequired)
            {
                comboBox.Invoke(new MethodInvoker(() =>
                {
                    comboBox.Items.Clear();
                    comboBox.Items.AddRange(items);
                    if (selectedIndex >= 0 && selectedIndex < items.Length)
                        comboBox.SelectedIndex = selectedIndex;
                }));
            }
            else
            {
                comboBox.Items.Clear();
                comboBox.Items.AddRange(items);
                if (selectedIndex >= 0 && selectedIndex < items.Length)
                    comboBox.SelectedIndex = selectedIndex;
            }
        }
    }
}