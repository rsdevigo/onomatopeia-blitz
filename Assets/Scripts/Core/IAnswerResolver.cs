namespace Blitz.Core
{
    public interface IAnswerResolver
    {
        SoundObjectId Resolve(GeneratedCard card, ActiveOnomatopoeiaSet activeSet);
    }
}
