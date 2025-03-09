import Core.Entity;

/**Simple wrapper fdor creating an entity that has no features other than existing */
public class Dummy extends Entity {

    public Dummy() {
        super();
    }
    public Dummy (String name) {
        super (name);
    }
}