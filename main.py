from skimage import transform
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import torch
import torch.nn.functional as F
from torch.utils.data import DataLoader
from floortrans.models import get_model
from floortrans.loaders import FloorplanSVG, DictToTensor, Compose, RotateNTurns
from floortrans.plotting import segmentation_plot, polygons_to_image, draw_junction_from_dict, discrete_cmap
discrete_cmap()
from floortrans.post_prosessing import split_prediction, get_polygons, split_validation
from mpl_toolkits.axes_grid1 import AxesGrid

import argparse
from utils import load_image
import csv
from PIL import Image

n_classes = 44
n_rooms = 12
rot = RotateNTurns()
room_classes = ["Background", "Outdoor", "Wall", "Kitchen", "Living Room" ,"Bed Room", "Bath", "Entry", "Railing", "Storage", "Garage", "Undefined"]

def main(args):
    model = get_model('hg_furukawa_original', 51)

    split = [21, 12, 11]
    model.conv4_ = torch.nn.Conv2d(256, n_classes, bias=True, kernel_size=1)
    model.upsample = torch.nn.ConvTranspose2d(n_classes, n_classes, kernel_size=4, stride=4)
    checkpoint = torch.load('model_best_val_loss_var.pkl')

    model.load_state_dict(checkpoint['model_state'])
    model.eval()
    model.cuda()
    
    raster_floor_plan : torch.Tensor = load_image(args.image, add_batch_dimension=True)
    raster_floor_plan = raster_floor_plan.cuda()

    (prediction, img_size) = model_forward(raster_floor_plan, model)
    
    heatmaps, rooms, icons = split_prediction(prediction, img_size, split)
    polygons, types, room_polygons, room_types, walls = get_polygons((heatmaps, rooms, icons), 0.2, [1, 2])
    
    pol_room_seg, pol_icon_seg = polygons_to_image(polygons, types, room_polygons, room_types, img_size[0], img_size[1])
    
    pol_room_seg = pol_room_seg.astype(np.uint8)
    image_save = Image.fromarray(pol_room_seg)
    image_save.save("revit_wall.png")
    
    walls_list = walls.tolist()
    
    with open(args.output, 'w', newline='') as csvfile:
        writer = csv.writer(csvfile, delimiter=',')
        writer.writerows(walls_list)


def model_forward(image: torch.Tensor, model):
    with torch.no_grad():
        height = image.shape[2]
        width = image.shape[3]
        img_size = (height, width)
        
        rotations = [(0, 0), (1, -1), (2, 2), (-1, 1)]
        pred_count = len(rotations)
        prediction = torch.zeros([pred_count, n_classes, height, width])
        for i, r in enumerate(rotations):
            forward, back = r
            # We rotate first the image
            rot_image = rot(image, 'tensor', forward)
            pred = model(rot_image)
            # We rotate prediction back
            pred = rot(pred, 'tensor', back)
            # We fix heatmaps
            pred = rot(pred, 'points', back)
            # We make sure the size is correct
            pred = F.interpolate(pred, size=(height, width), mode='bilinear', align_corners=True)
            # We add the prediction to output
            prediction[i] = pred[0]
            
    prediction = torch.mean(prediction, 0, True)
    return prediction, img_size

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--image', nargs='?', type=str, default="floor_plan.png")
    parser.add_argument('--output', nargs='?', type=str, default="revit_wall.csv")
    args = parser.parse_args()
    
    main(args)
