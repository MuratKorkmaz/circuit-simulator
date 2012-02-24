using System.Windows.Forms;

namespace JavaToSharp
{
    class UserMessageView
    {
        private static UserMessageView _instance;

        public static UserMessageView Instance
        {
            get { return _instance ?? (_instance = new UserMessageView()); }
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool ShowQuestion(string messege)
        {
            var dialog = MessageBox.Show(messege, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            bool isYesAnswer = dialog == DialogResult.Yes;
            return isYesAnswer;
        }
    }
}
