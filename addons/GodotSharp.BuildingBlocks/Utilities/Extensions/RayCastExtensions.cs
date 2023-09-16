using System.Diagnostics;
using Godot;
using Godot.Collections;

namespace GodotSharp.BuildingBlocks
{
    public static class RayCastExtensions
    {
        public static bool CastRay(this Node3D node, in Vector3 direction, float distance, Action collision) => node.CastRay(node.Position, direction, distance, collision);
        public static bool CastRay(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance, Action collision)
        {
            if (RayCollision(node, rayStart, direction, distance, out var _))
            {
                node.CallDeferred(collision);
                return true;
            }

            return false;
        }

        public static bool CastRay(this Node3D node, in Vector3 direction, float distance, Action<CollisionObject3D> collision) => node.CastRay(node.Position, direction, distance, collision);
        public static bool CastRay(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance, Action<CollisionObject3D> collision)
        {
            if (RayCollision(node, rayStart, direction, distance, out var result))
            {
                var collisionObject = (CollisionObject3D)result["collider"].AsGodotObject();
                node.CallDeferred(() => collision(collisionObject));
                return true;
            }

            return false;
        }

        public static bool CastRay(this Node3D node, in Vector3 direction, float distance, Action<CollisionObject3D, Vector3, Vector3> collision) => node.CastRay(node.Position, direction, distance, collision);
        public static bool CastRay(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance, Action<CollisionObject3D, Vector3, Vector3> collision)
        {
            if (RayCollision(node, rayStart, direction, distance, out var result))
            {
                var surfaceNormal = result["normal"].AsVector3();
                var intersectPoint = result["position"].AsVector3();
                var collisionObject = (CollisionObject3D)result["collider"].AsGodotObject();

                node.CallDeferred(() => collision(collisionObject, intersectPoint, surfaceNormal));
                return true;
            }

            return false;
        }

        public static bool CastRay(this Node3D node, in Vector3 direction, float distance, Action<CollisionObject3D, CollisionShape3D, Vector3, Vector3> collision) => node.CastRay(node.Position, direction, distance, collision);
        public static bool CastRay(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance, Action<CollisionObject3D, CollisionShape3D, Vector3, Vector3> collision)
        {
            if (RayCollision(node, rayStart, direction, distance, out var result))
            {
                var surfaceNormal = result["normal"].AsVector3();
                var intersectPoint = result["position"].AsVector3();
                var collisionObject = (CollisionObject3D)result["collider"].AsGodotObject();

                var shapeIndex = result["shape"].AsInt32();
                var shapeOwner = collisionObject.ShapeFindOwner(shapeIndex);
                var collisionShape = (CollisionShape3D)collisionObject.ShapeOwnerGetOwner(shapeOwner);

                node.CallDeferred(() => collision(collisionObject, collisionShape, intersectPoint, surfaceNormal));
                return true;
            }

            return false;
        }

        private static bool RayCollision(Node3D node, in Vector3 rayStart, in Vector3 direction, float distance, out Dictionary result)
        {
            Debug.Assert(Engine.IsInPhysicsFrame());

            var rayEnd = rayStart + direction * distance;
            var query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd,
                exclude: node is CollisionObject3D collider ? new() { collider.GetRid() } : null);
            result = node.GetWorld3D().DirectSpaceState.IntersectRay(query);
            return result.Count is not 0;
        }
    }
}
