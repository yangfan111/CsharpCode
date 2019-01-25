对于可破碎物体使其爆破的常用方法：
1. 使用带刚体组件的物体撞击
2. 对于机枪无子弹射击破碎的情况使用如下代码：
    RaycastHit hitInfo;
    FracturedChunk chunkRaycast = FracturedChunk.ChunkRaycast(transform.position, transform.forward, out hitInfo);
    if(chunkRaycast)
    {
        // Hit it!
        chunkRaycast.Impact(hitInfo.point, ExplosionForce, ExplosionRadius, true);
    }
3. 对于想从物体内部爆破的情况使用如下类似代码：
    fracturedObject.Explode(transform.position, Force, InfluenceRadius, false, true, false, CheckStructureIntegrity);