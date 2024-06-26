﻿using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    /// <summary>
    /// A sensor used to control a humanoid
    /// </summary>
    [System.Serializable]
    public abstract class HumanoidSensor : Sensor {

        public HumanoidSensor() {
            enabled = true;
        }

        public new virtual HumanoidTracker tracker => null;

        //protected Tracking.Sensor sensor;

        [System.NonSerialized]
        public const string _name = "";
        public override string name { get { return _name; } }


        public Vector3 sensor2TargetPosition;
        public Quaternion sensor2TargetRotation;

        #region Start


        public virtual void Start(HumanoidControl _humanoid, Transform targetTransform) {
            target = targetTransform.GetComponent<Target>();
        }

        public virtual void CheckSensorTransform() {
            if (enabled && sensorComponent == null)
                CreateSensorTransform();
            else if (!enabled && sensorComponent != null)
                RemoveSensorTransform();

            if (sensor2TargetRotation.x + sensor2TargetRotation.y + sensor2TargetRotation.z + sensor2TargetRotation.w == 0)
                SetSensor2Target();
        }

        protected virtual void CreateSensorTransform() {
        }

        protected void CreateSensorTransform(Transform targetTransform, string resourceName, Vector3 _sensor2TargetPosition, Quaternion _sensor2TargetRotation) {
            GameObject sensorObject;
            if (resourceName == null) {
                sensorObject = new GameObject("Sensor");
            }
            else {
                Object controllerPrefab = Resources.Load(resourceName);
                if (controllerPrefab == null)
                    sensorObject = new GameObject("Sensor");
                else
                    sensorObject = (GameObject)Object.Instantiate(controllerPrefab);

                sensorObject.name = resourceName;
            }

            //sensorTransform = sensorObject.transform;
            //tracker.trackerComponent = tracker.GetTrackerComponent();
            sensorObject.transform.parent = tracker.trackerComponent.transform;

            sensor2TargetPosition = -_sensor2TargetPosition;
            sensor2TargetRotation = Quaternion.Inverse(_sensor2TargetRotation);

            UpdateSensorTransformFromTarget(targetTransform);
        }

        protected void RemoveSensorTransform() {
            if (Application.isPlaying)
                Object.Destroy(sensorComponent.gameObject);
            else
                Object.DestroyImmediate(sensorComponent.gameObject, true);
        }

        public virtual void SetSensor2Target() {
            if (sensorComponent == null || target == null)
                return;

            sensor2TargetRotation = Quaternion.Inverse(sensorComponent.transform.rotation) * target.transform.rotation;
            sensor2TargetPosition = -InverseTransformPointUnscaled(target.transform, sensorComponent.transform.position);
        }

        public static Vector3 InverseTransformPointUnscaled(Transform transform, Vector3 position) {
            var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            return worldToLocalMatrix.MultiplyPoint3x4(position);
        }
        #endregion

        #region Update
        public override void Update() {
            if (tracker == null || !tracker.enabled || !enabled)
                return;

            //if (sensor == null)
            //    return;

            //sensor.Update();
            //if (sensor.status != Tracker.Status.Tracking)
            //    return;

            //UpdateSensorTransform(sensor);
            UpdateTargetTransform();
        }

        protected void UpdateSensorTransform(Tracking.Sensor sensor) {
            if (sensorComponent == null)
                return;

            if (status == Tracker.Status.Tracking) {
                sensorComponent.gameObject.SetActive(true);
                sensorComponent.transform.position = HumanoidTarget.ToVector3(sensor.sensorPosition);
                sensorComponent.transform.rotation = HumanoidTarget.ToQuaternion(sensor.sensorRotation);
            }
            else {
                sensorComponent.gameObject.SetActive(false);
            }
        }

        public virtual void UpdateSensorTransformFromTarget(Transform targetTransform) {
            if (sensorComponent != null) {
                sensorComponent.transform.position = TransformPointUnscaled(targetTransform, -sensor2TargetPosition);
                sensorComponent.transform.rotation = targetTransform.rotation * Quaternion.Inverse(sensor2TargetRotation);
                return;
            }
            
            if (sensorComponent == null)
                return;

            sensorComponent.transform.position = TransformPointUnscaled(targetTransform, -sensor2TargetPosition);
            sensorComponent.transform.rotation = targetTransform.rotation * Quaternion.Inverse(sensor2TargetRotation);
        }

        protected static Vector3 TransformPointUnscaled(Transform transform, Vector3 position) {
            var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            return localToWorldMatrix.MultiplyPoint3x4(position);
        }

        protected virtual void UpdateTargetTransform() {
            target.transform.rotation = sensorComponent.transform.rotation * sensor2TargetRotation;
            target.transform.position = sensorComponent.transform.position + target.transform.rotation * sensor2TargetPosition;
        }
        #endregion

        #region Stop
        public virtual void Stop() { }
        #endregion

        public virtual void RefreshSensor() {
        }

        public virtual void ShowSensor(HumanoidTarget target, bool shown) { }
        //}

        //public class HumanoidSensor : UnitySensor {

        protected virtual void UpdateTarget(HumanoidTarget.TargetTransform target, Transform sensorTransform) {
            if (target.transform == null || sensorTransform == null)
                return;

            target.transform.rotation = GetTargetRotation(sensorTransform);
            target.confidence.rotation = 0.5F;

            target.transform.position = GetTargetPosition(sensorTransform);
            target.confidence.position = 0.5F;
        }

        protected virtual void UpdateTarget(HumanoidTarget.TargetTransform target, SensorComponent sensorComponent) {
            if (target == null || target.transform == null ||
                sensorComponent == null || sensorComponent.rotationConfidence + sensorComponent.positionConfidence <= 0)
                return;

            target.transform.rotation = GetTargetRotation(sensorComponent.transform);
            target.confidence.rotation = sensorComponent.rotationConfidence;

            target.transform.position = GetTargetPosition(sensorComponent.transform);
            target.confidence.position = sensorComponent.positionConfidence;
        }

        protected Vector3 GetTargetPosition(Transform sensorTransform) {
            Vector3 targetPosition = sensorTransform.position + sensorTransform.rotation * sensor2TargetRotation * sensor2TargetPosition;
            return targetPosition;
        }

        protected Quaternion GetTargetRotation(Transform sensorTransform) {
            Quaternion targetRotation = sensorTransform.rotation * sensor2TargetRotation;
            return targetRotation;
        }
    }

}