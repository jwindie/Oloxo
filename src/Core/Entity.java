package Core;

import java.util.HashMap;
import java.util.Map;

public abstract class Entity {
    private int instanceId;
    private String name;

    public Entity() {
        instanceId = createdObjects++;
        name = "" + instanceId;
        entityIdMap.put(instanceId, this);
        entityNameMap.put(name, this);
    }
    protected Entity(String name) {
        instanceId = createdObjects++;
        entityIdMap.put(instanceId, this);
        entityNameMap.put(name, this);

        if (name == null || name.isEmpty()) this.name = "" + instanceId;
        else this.name = name;
    }

    public int getInstanceId() {
        return instanceId;
    }
    public String getName() {
        return name;
    }

    public void setName(String name) {
        entityNameMap.remove(this.name);
        this.name  = name;
        entityNameMap.put(name, this);
    }

    public void destroy() {
        entityIdMap.remove(instanceId);
        entityNameMap.remove(name);
    }

    //statics
    public static int getCount() {
        return entityIdMap.size();
    }
    public static int[] getIds() {
        //this works similarly to System.Linq in that is converts the 
        //entity map over into an array of integers to then send back to the user
        return entityIdMap.keySet().stream()
            .mapToInt(Integer::intValue)
            .sorted()
            .toArray()
        ;
    }
    public static String[] getNames() {
        //this works similarly to System.Linq in that is converts the 
        //entity map over into an array of strings to then send back to the user
        return entityNameMap.keySet().stream()
            .sorted() // Sort alphabetically
            .toArray(String[]::new)
        ; // Convert to String[]
    }
    public static boolean exists (int id) {
        return entityIdMap.containsKey(id);
    }
    public static boolean exists (String name) {
        return entityNameMap.containsKey(name);
    }
    public static Entity get(int id) {
        if (entityIdMap.containsKey(id)) return entityIdMap.get(id);
        else return null;
    }
    public static Entity get(String name) {
        if (entityNameMap.containsKey(name)) return entityNameMap.get(name);
        else return null;
    }
    private static Map<Integer, Entity> entityIdMap = new HashMap<>();
    private static Map<String, Entity> entityNameMap = new HashMap<>();
    private static int createdObjects = 0;
}