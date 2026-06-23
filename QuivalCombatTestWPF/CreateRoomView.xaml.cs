using System.Windows.Controls;
using System.Windows.Input;

namespace QuivalCombatTestWPF
{
    public partial class CreateRoomView : UserControl
    {
        public CreateRoomView()
        {
            InitializeComponent();
            RoomNameTextBox.KeyDown += RoomNameTextBox_KeyDown;
        }

        private void RoomNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //TODO: connect to server and create the room
                //Then maybe jump to some kind of waiting for player room screen

                var success = QuivalClient.SubmitRoomCreationRequest(RoomNameTextBox.Text);
                if (success)
                {
                    //do something
                }
            }
        }
    }
}
