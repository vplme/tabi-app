using System.Collections.Generic;
using System.Threading.Tasks;
using Tabi.DataObjects;
using TabiApiClient.Messages;

namespace TabiApiClient
{
    public interface IApiClient
    {
        Task<TokenResult> Authenticate(string username, string password);
        Task<bool> Register(UserMessage user);
        Task<DeviceMessage> GetDevice(int id);
        Task<DeviceCounts> GetDeviceCounts(int id);

        Task<DeviceMessage> RegisterDevice(string model = "",
            string os = "", string osVersion = "",
            string manufacturer = "");

        Task<bool> PostPositions(int deviceId, List<PositionEntry> positions);
        Task<bool> PostStops(int deviceId, List<Models.Stop> stops);
        Task<bool> PostUserStopMotives(int deviceId, List<Models.UserStopMotive> motives);
        Task<bool> PostStopVisits(int deviceId, List<Models.StopVisit> stopVisits);
        Task<bool> PostLogs(int deviceId, List<Models.Log> logs);
        Task<bool> PostBatteryData(int deviceId, List<Models.BatteryInfo> batteryEntries);
        Task<bool> PostTrackEntries(int deviceId, List<Models.TrackEntry> trackEntries);
        Task<bool> PostTransportationModes(int deviceId, IEnumerable<Models.TransportationMode> transportModes);
        Task<bool> PostSensorMeasurementSessions(int deviceId, List<SensorMeasurementSession> sensorMeasurementSessions);
        Task<bool> PostAccelerometerData(int deviceId, List<Accelerometer> accelerometerData);
        Task<bool> PostGyroscopeData(int deviceId, List<Gyroscope> gyroscopeData);
        Task<bool> PostMagnetometerData(int deviceId, List<Magnetometer> magnetometerData);
        Task<bool> PostLinearAccelerationData(int deviceId, List<LinearAcceleration> linearAcceleration);
        Task<bool> PostGravityData(int deviceId, List<Gravity> gravity);
        Task<bool> PostQuaternionData(int deviceId, List<Quaternion> quaternionData);
        Task<bool> PostOrientationData(int deviceId, List<Orientation> orientationData);
        Task<bool> IsDeviceUnauthorized(int deviceId);
    }
}