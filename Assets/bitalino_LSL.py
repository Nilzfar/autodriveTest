import time
import atexit
from bitalino import BITalino
from pylsl import StreamInfo, StreamOutlet, local_clock

print("Friedrich-Alexander-University fo Erlangen-Nürnberg (FAU)")
print("Department of Medical Informatics, Biometry and Epidemiology")
print("Niloufar.Ramezanzadegan@fau.de")

# device MAC Address
macAddress = "98:D3:91:FD:3E:FC"

batteryThreshold = 30
acqChannels = [3]
samplingRate = 1000
nSamples = 1
digitalOutput = [1,1]

# Connect to BITalino
device = BITalino(macAddress)

def exit_handler():
    # Stop acquisition
    device.stop()  
    # Close connection
    device.close()
    print('connection closed.')

atexit.register(exit_handler)

# Set battery threshold
device.battery(batteryThreshold)

# Read BITalino version
print(device.version())

# creating stream
srate = samplingRate
name = 'Bitalino'
type = 'EDA'
n_channels = len(acqChannels)
info = StreamInfo(name=name, 
                    type=type,
                    channel_count=n_channels,
                    nominal_srate=srate,
                    channel_format='float32',
                    source_id=macAddress)

outlet = StreamOutlet(info)

# Start Acquisition
device.start(samplingRate, acqChannels)
print('now acquising data ...')

start_time = local_clock()
sent_samples = 0

while True:
    mysample = device.read(nSamples)[:, -1]
    #print(mysample)
    outlet.push_sample(mysample)

    time.sleep(0.01)
