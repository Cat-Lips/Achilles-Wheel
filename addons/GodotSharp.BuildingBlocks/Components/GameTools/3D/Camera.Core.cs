using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class CameraCore
    {
        public static void OnSelect(this Camera3D camera, Action<CollisionObject3D> onObjectSelected)
        {
            if (onObjectSelected is null) return;

            var screenPos = camera.GetViewport().GetMousePosition();
            var rayStart = camera.ProjectRayOrigin(screenPos);
            var rayNormal = camera.ProjectRayNormal(screenPos);

            camera.CastRay(rayStart, rayNormal, camera.Far, onObjectSelected);
        }

        public static void OnFreeLookUpdate(this Camera3D camera, float speed, float delta)
        {
            if (Input.IsActionPressed(MyInput.Boost))
                speed *= 10;

            var (x, z) = Input.GetVector(MyInput.Left, MyInput.Right, MyInput.Forward, MyInput.Back) * speed * delta;
            var y = Input.GetAxis(MyInput.Down, MyInput.Up) * speed * delta;
            camera.TranslateObjectLocal(new(x, y, z));
        }

        public static void OnChaseCamUpdate(this Camera3D camera, RigidBody3D target, Vector3 offset, float speed, float delta)
            => camera.OnFollowCamUpdate(target, offset, speed, delta); // Same (for now)

        public static void OnFollowCamUpdate(this Camera3D camera, Node3D target, Vector3 offset, float speed, float delta)
        {
            var sourceTransform = camera.Transform;
            var targetTransform = target.Transform.TranslatedLocal(offset);
            camera.Transform = sourceTransform.InterpolateWith(targetTransform, speed * delta);
            //camera.LookAt(target.Position);
        }

        private static readonly Dictionary<Camera3D, Vector3> CurrentRotation = [];
        public static void OnFreeLookUpdate(this Camera3D camera, InputEvent e, float sensitivity, float minAngle, float maxAngle)
        {
            if (e is InputEventMouseMotion motion)
            {
                if (!CurrentRotation.TryGetValue(camera, out var currentRotation))
                    CurrentRotation.Add(camera, currentRotation = new());

                var (x, y) = -motion.Relative * sensitivity;

                currentRotation.X += x;
                currentRotation.Y += y;

                ClampVerticalOrientation();

                var transform = camera.Transform;
                transform.Basis = Basis.Identity;
                camera.Transform = transform;

                camera.RotateY(currentRotation.X);
                camera.RotateX(currentRotation.Y);

                void ClampVerticalOrientation()
                {
                    currentRotation.Y = Clamp(currentRotation.Y);

                    float Clamp(float y)
                        => Math.Clamp(y, Mathf.DegToRad(minAngle), Mathf.DegToRad(maxAngle));
                }
            }
        }

        public static void OnChaseCamUpdate(this Camera3D camera, InputEvent e, RigidBody3D body, float sensitivity, float minAngle, float maxAngle)
        {
            // TODO
        }

        public static void OnFollowCamUpdate(this Camera3D camera, InputEvent e, Node3D target, float sensitivity, float minAngle, float maxAngle)
        {
            // TODO
        }
    }
}
