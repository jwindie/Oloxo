namespace Oloxo.Data {
    public interface ISerialize {
        public object[] Serialize ();
        public void Deserialize (object[] data);
    }
}
