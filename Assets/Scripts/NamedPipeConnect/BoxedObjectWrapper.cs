namespace NamedPipeConnection
{
    public class BoxedObjectWrapper
    {
        public object BoxedObject { get; private set; }

        public BoxedObjectWrapper(object boxedObject)
        {
            BoxedObject = boxedObject;
        }
    }
}