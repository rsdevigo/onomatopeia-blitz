namespace Blitz.Core
{
    public interface IAnswerResolver
    {
        SoundObjectId Resolve(GeneratedCard card, ActiveLetterSoundSet activeSet);
    }
}
