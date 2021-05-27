public interface IMsgPacker
{
    void Pack(object msg);
    object Unpack();
}