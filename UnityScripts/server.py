#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import time
import zmq
import numpy as np
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")


start = socket.recv()
socket.send(b"start recived")
height = socket.recv()
height = int.from_bytes(height, "big") 
socket.send(b"recived height")
width = socket.recv()
width = int.from_bytes(width, "big") 
socket.send(b"recived width")
print("height" + str(height) + "width" + str(width))


while True:
    #  Wait for next request from client
    frame = socket.recv()
    print("Frame request")
    img = np.frombuffer(frame, dtype=np.uint8).reshape((height, width, 4))
    image_without_alpha = img[:,:,:3];
    #  Do some 'work'.
    #  Try reducing sleep time to 0.01 to see how blazingly fast it communicates
    #  In the real world usage, you just need to replace time.sleep() with
    #  whatever work you want python to do, maybe a machine learning task?
    
    print(image_without_alpha)
    x = 2
    str1 =""
    cooridnates = [1,2,3,4,86,88,101,200]
    cooridnates.insert(0, x)
    str1 = ' '.join(str(e) for e in cooridnates)
    print(str1)
    arr = bytes(str1, 'utf-8')
    print(arr)
    #  Send reply back to client
    #  In the real world usage, after you finish your work, send your output here
    
    socket.send(arr)
