
import cv2 as cv2
import numpy as np


def random_colors(N):
    np.random.seed(1)
    colors = [tuple(255 * np.random.rand(3)) for _ in range(N)]
    return colors

def apply_mask(image, mask, color, alpha=0.5):
    """apply mask to image"""
    for n, c in enumerate(color):
        image[:, :, n] = np.where(
            mask == 1,
            image[:, :, n] * (1 - alpha) + alpha * c,
            image[:, :, n]
        )
    return image


def display_instances(image, boxes, masks, ids, names, scores):
    
    n_instances = boxes.shape[0]
        
    if n_instances:
        assert boxes.shape[0] == masks.shape[-1] == ids.shape[0]
    
    colors = random_colors(n_instances)
    height, width = image.shape[:2]
    cooridnates = []
    x = 0
    for i, color in enumerate(colors):
        if not np.any(boxes[i]):
            continue
        
        y1, x1, y2, x2 = boxes[i]
        cooridnates.append(y1)
        cooridnates.append(x1)
        cooridnates.append(y2)
        cooridnates.append(x2)
        mask = masks[:, :, i]
        x = x +1
        #image = apply_mask(image, mask, color)
        #image = cv2.rectangle(image, (x1, y1), (x2, y2), color, 2)
        
        #label = names[ids[i]]
        #score = scores[i] if scores is not None else None
        #caption = '{} {:.2f}'.format(label, score) if score else label 
        #image = cv2.putText(
            #image, caption, (x1, y1), cv2.FONT_HERSHEY_COMPLEX, 0.7, color, 2
        #)
    return x, cooridnates

if __name__ == '__main__':
    import os
    import sys
    import random
    import math
    import time
    import balloon
    import utils
    import model as modellib
    #socket binding to unity binding to socket 5555
    import zmq
    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind("tcp://*:5555")
        
    
    ROOT_DIR = os.getcwd()
    MODEL_DIR = os.path.join(ROOT_DIR, "logs")
    SODA_WEIGHTS_PATH = os.path.join(ROOT_DIR, "mask_rcnn_soda_can.h5")
    COCO_MODEL_PATH = os.path.join(ROOT_DIR,"mask_rcnn_coco.h5")
    if not os.path.exists(SODA_WEIGHTS_PATH):
        utils.download_trained_weights(SODA_WEIGHTS_PATH)
    
    config = balloon.BalloonConfig()
    class InferenceConfig(config.__class__):
        GPU_COUNT = 1
        IMAGES_PER_GPU = 1
        
    config = InferenceConfig()
    config.display()
    
    model = modellib.MaskRCNN(
        mode="inference" , model_dir=MODEL_DIR, config=config
    )
    
  
    model.load_weights(SODA_WEIGHTS_PATH, by_name=True)
    class_names = ['BG', 'soda_can']
    
    
    #capture = cv2.VideoCapture(0)
    #capture.set(cv2.CAP_PROP_FRAME_WIDTH, 1920)
    #capture.set(cv2.CAP_PROP_FRAME_HEIGHT, 1080)
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
        # !! socket work
        
        
        frame = socket.recv()
        print("Received frame" )
        
        
        img = np.frombuffer(frame, dtype=np.uint8).reshape((480, 640, 4))
        image_without_alpha = img[:,:,:3];
        # end socket work
        # normal config right now for model 
        startTime = time.time()
        #ret, frame = capture.read()
        
        captureTime = time.time()
        results = model.detect([image_without_alpha], verbose=0)
        modelTime = time.time()
        r = results[0]
        
        x, cooridnates = display_instances(
            image_without_alpha, r['rois'], r['masks'], r['class_ids'], class_names, r['scores']
        )
        
        str1 =""
        
        for i in range(0,x):
            cooridnates.insert(4 * (i+1), r['scores'][i])
        cooridnates.insert(0, x)
        str1 = ' '.join(str(e) for e in cooridnates)
        print(str1)
        arr = bytes(str1, 'utf-8')
        print(arr)
        
        socket.send(arr)
        renderTime = time.time()
        
        
        
    
    #caption.release()
    #cv2.destoryWindow('frame')
    
    print("Start Time: "+ str(startTime))
    timeTocap = captureTime - startTime
    print("Time it took to capture the frame: " + str(timeTocap))
    timeTomod = modelTime - captureTime
    print("Time it took for the model to run: " + str(timeTomod))
    timeToren = renderTime - modelTime
    print("Time it took for the image to render: " + str(timeToren))
    timeToshow = showTime - renderTime
    print("Time it took for the image to render: " + str(timeToshow))
    
        
        
        
    