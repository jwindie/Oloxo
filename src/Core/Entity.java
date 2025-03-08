package Griddom.Core;

import java.util.HashMap;
import java.util.Map;

/**Represents any Pigl object. */
public class Entity {
    private static Map<Integer, Entity> entityMap = new HashMap<>();
    private static int createdObjects = 0;
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
}