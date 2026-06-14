public class NPCDemo1 : NPCBase
{
    public override bool TryInteract(out InteractionResult result)
    {
        result = new OpenDialogueResult(_npcName);

        return CanInteract;
    }
}