function ScaleImage(imgFileName)

	coreName = imgFileName(1:length(imgFileName)-3-1);
	img = imread(imgFileName);
	
	scale = 0.95;
	minHeight = 40;

    scales = [];
    idx=1;
    lastScaledHeight = size(img, 1);
    while lastScaledHeight >= minHeight
        
        lastScaledHeight = lastScaledHeight * scale;
        scales(idx) = lastScaledHeight / size(img, 1);
        idx = idx + 1;
    end
    
    idx=1;
    for s = scales
        imR = imresize(img, scales(idx));
		imwrite(imR, strcat(coreName, '_Resized_', num2str(idx), '.bmp'));
        idx = idx + 1;
    end
end