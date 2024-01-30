using PenguinParty.Packets;

namespace PenguinParty.Services;

public interface IPenguinPartyService
{
    public void RoundStart();
    public void RoundEnd();
    public void AutoSkipTurn();
    public void Start(ulong[] userIds);
    public void SubmitCard(SubmitCardRequest request);
}