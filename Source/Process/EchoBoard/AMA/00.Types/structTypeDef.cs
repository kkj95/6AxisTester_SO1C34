namespace FZ4P
{
    public class AMA_TestSetting_Params : Echo_ParamBase
    {
        private int ois_mode;
        private int target_slave_id_X;
        private int target_slave_id_Y;
        private int target_slave_id_Z;
        private int clock_devision;
        private int eOIS_target_device_number;
        private int af_target_device_number;
        private int set_read_address;
        private int read_address_count;
        private int frequency;
        private int amplitude;
        private int threshold;
        private int measurement_cycle_count;
        private int dummy_cycle_count;

        public int Ois_mode { get => ois_mode; set => ois_mode = value; }
        public int Clock_devision { get => clock_devision; set => clock_devision = value; }
        public int EOIS_target_device_number { get => eOIS_target_device_number; set => eOIS_target_device_number = value; }
        public int Af_target_device_number { get => af_target_device_number; set => af_target_device_number = value; }
        public int Set_read_address { get => set_read_address; set => set_read_address = value; }
        public int Read_address_count { get => read_address_count; set => read_address_count = value; }
        public int Frequency { get => frequency; set => frequency = value; }
        public int Amplitude { get => amplitude; set => amplitude = value; }
        public int Threshold { get => threshold; set => threshold = value; }
        public int Measurement_cycle_count { get => measurement_cycle_count; set => measurement_cycle_count = value; }
        public int Dummy_cycle_count { get => dummy_cycle_count; set => dummy_cycle_count = value; }
        public int Target_slave_id_X { get => target_slave_id_X; set => target_slave_id_X = value; }
        public int Target_slave_id_Y { get => target_slave_id_Y; set => target_slave_id_Y = value; }
        public int Target_slave_id_Z { get => target_slave_id_Z; set => target_slave_id_Z = value; }
    }

    public class AMA_RingingSetting_Params : Echo_ParamBase
    {
        private int target_slave_id_X;
        private int target_slave_id_Y;
        private int target_slave_id_Z;
        private int clock_devision;
        private int eOIS_target_device_number;
        private int af_target_device_number;
        private int end_positionx;
        private int start_positionx;
        private int end_positiony;
        private int start_positiony;
        private int start_time;
        private int end_time;
        private int threshold;

        public int Target_slave_id_X { get => target_slave_id_X; set => target_slave_id_X = value; }
        public int Target_slave_id_Y { get => target_slave_id_Y; set => target_slave_id_Y = value; }
        public int Target_slave_id_Z { get => target_slave_id_Z; set => target_slave_id_Z = value; }
        public int Clock_devision { get => clock_devision; set => clock_devision = value; }
        public int EOIS_target_device_number { get => eOIS_target_device_number; set => eOIS_target_device_number = value; }
        public int Af_target_device_number { get => af_target_device_number; set => af_target_device_number = value; }

        public int Start_time { get => start_time; set => start_time = value; }
        public int End_time { get => end_time; set => end_time = value; }
        public int Threshold { get => threshold; set => threshold = value; }

        public int End_positionX { get => end_positionx; set => end_positionx = value; }
        public int Start_positionX { get => start_positionx; set => start_positionx = value; }
        public int End_positionY { get => end_positiony; set => end_positiony = value; }
        public int Start_positionY { get => start_positiony; set => start_positiony = value; }
    }


    public class SineResult 
    {
        private int _deltaMaxX = -1;
        private int _deltaMaxY = -1;
        private int _ngCountX = -1;
        private int _ngCountY = -1;

        public int DeltaMaxX 
        { 
            get => _deltaMaxX; 
            set => _deltaMaxX = value; 
        }
        public int DeltaMaxY 
        { 
            get => _deltaMaxY; 
            set => _deltaMaxY = value; 
        }
        public int NgCountX 
        { 
            get => _ngCountX; 
            set => _ngCountX = value; 
        }
        public int NgCountY 
        { 
            get => _ngCountY; 
            set => _ngCountY = value; 
        }
    }
    public class RingingResult
    {
        private int okCountX = -1;
        private int okCountY = -1;

        private int settlingTimeX = -1;
        private int settlingTimeY = -1;

        public int OkCountX { get => okCountX; set => okCountX = value; }
        public int OkCountY { get => okCountY; set => okCountY = value; }
        public int SettlingTimeX { get => settlingTimeX; set => settlingTimeX = value; }
        public int SettlingTimeY { get => settlingTimeY; set => settlingTimeY = value; }
    }

}
