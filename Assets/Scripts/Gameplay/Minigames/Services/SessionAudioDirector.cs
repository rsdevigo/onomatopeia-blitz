using Blitz.Gameplay.Content;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    public sealed class SessionAudioDirector : IAudioDirector
    {
        readonly AudioSource? _source;

        public SessionAudioDirector(AudioSource? source) => _source = source;

        public void PlayCardCue(OnomatopoeiaDefinition definition)
        {
            if (definition?.AudioClip is null)
                return;

            if (_source != null)
                _source.PlayOneShot(definition.AudioClip);
            else
                AudioSource.PlayClipAtPoint(definition.AudioClip, Vector3.zero);
        }
    }
}
