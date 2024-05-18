import torch
from torchvision import transforms
from PIL import Image

def load_image(image_path, add_batch_dimension = False):
    # Load image using PIL
    image = Image.open(image_path).convert('RGB')
    
    # Transform the image to tensor
    transform = transforms.ToTensor()
    tensor_image = transform(image)

    if add_batch_dimension:
        tensor_image = tensor_image.unsqueeze(0)
        
    return tensor_image