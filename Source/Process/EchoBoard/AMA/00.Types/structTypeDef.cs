namespace FZ4P
{
    public class AMA_TestSetting_Params : Echo_ParamBase
    {
        private byte ois_mode;
        private byte target_slave_id_X;
        private byte target_slave_id_Y;
        private byte target_slave_id_Z;
        private byte clock_devision;
        private byte eOIS_target_device_number;
        private byte af_target_device_number;
        private byte set_read_address;
        private byte read_address_count;
        private byte frequency;
        private byte amplitude;
        private byte threshold;
        private byte measurement_cycle_count;
        private byte dummy_cycle_count;

        public byte Ois_mode { get => ois_mode; set => ois_mode = value; }

        public byte Clock_devision { get => clock_devision; set => clock_devision = value; }
        public byte EOIS_target_device_number { get => eOIS_target_device_number; set => eOIS_target_device_number = value; }
        public byte Af_target_device_number { get => af_target_device_number; set => af_target_device_number = value; }
        public byte Set_read_address { get => set_read_address; set => set_read_address = value; }
        public byte Read_address_count { get => read_address_count; set => read_address_count = value; }
        public byte Frequency { get => frequency; set => frequency = value; }
        public byte Amplitude { get => amplitude; set => amplitude = value; }
        public byte Threshold { get => threshold; set => threshold = value; }
        public byte Measurement_cycle_count { get => measurement_cycle_count; set => measurement_cycle_count = value; }
        public byte Dummy_cycle_count { get => dummy_cycle_count; set => dummy_cycle_count = value; }
        public byte Target_slave_id_X { get => target_slave_id_X; set => target_slave_id_X = value; }
        public byte Target_slave_id_Y { get => target_slave_id_Y; set => target_slave_id_Y = value; }
        public byte Target_slave_id_Z { get => target_slave_id_Z; set => target_slave_id_Z = value; }
    }

}
