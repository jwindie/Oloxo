package Core;

import java.util.HashMap;
import java.util.Map;

public class Entity {
    private int instanceId;

    public Entity() {
        instanceId = createdObjects++;
        entityMap.put(instanceId, this);
    }

    public static int getCount() {
        return entityMap.size();
    }

    public int getInstanceId() {
        return instanceId;
    }

    public void destroy() {
        entityMap.remove(instanceId);
    }

    //statics
    private static Map<Integer, Entity> entityMap = new HashMap<>();
    private static int createdObjects = 0;
}