using System;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "UserSettings", menuName = "Config/User Settings")]
    public class UserSettings : ScriptableObject
    {
        public event EventHandler Changed;

        [field: NonSerialized] public bool MusicOn { get; private set; } = true;
        [field: NonSerialized] public bool SoundFXOn { get; private set; } = true;

        public void ToggleMusic()
        {
            MusicOn = !MusicOn;
            Changed?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void ToggleSoundFX()
        {
            SoundFXOn = !SoundFXOn;
            Changed?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
