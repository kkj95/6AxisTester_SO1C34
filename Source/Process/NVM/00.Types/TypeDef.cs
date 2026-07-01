using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class NVMWriteRow
    {
        public int Address { get; set; } = 0;
        public int PlayLoad { get; set; } = 0;
    }
    public class NVMWriteCollection : List<NVMWriteRow>
    {
        public void AddRow(int address, int playLoad)
        {
            this.Add(new NVMWriteRow
            {
                Address = address,
                PlayLoad = playLoad
            });
        }

        public void CopyAddress(NVMWriteCollection sourceCollection)
        {
            this.Clear();
            foreach (var element in sourceCollection)
            {
                this.Add(new NVMWriteRow
                {
                    Address = element.Address,
                    PlayLoad = 0
                });
            }
        }
    }
}
