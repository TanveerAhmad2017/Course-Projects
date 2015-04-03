%Goal: visualize the similarity matrix of different cluster results


%preprocess data
%dataName = 'dataset1-KMean5';
%dataName = 'dataset1-EM';
%dataName = 'dataset1-DBS-0dot2';

%dataName = 'dataset2-KMean5';
%dataName = 'dataset2-EM';
%dataName = 'dataset2-DBS-0dot4';

dataName = 'dataset2-KM-seed100';

data1 = load(strcat('..\processed-data\',dataName,'.txt'));
sortedData1 =sortrows(data1, 6);
sortedData1 = sortedData1(:,1:5);

instanceNum = size(sortedData1,1);

%euclidean distance
proximity = pdist2(sortedData1,sortedData1);
proximity = 1 - (proximity)/max(max(proximity));

%set figure
figure1 = figure;
axes1 = axes('Parent',figure1);
set(axes1,'FontSize',26,'FontWeight','bold');


%plot figure
imagesc(proximity)

colorbar

% set(gca,'FontSize', 26);

%set x, y Label
%set(axes1,'YTick',[100:100:700],'YTickLabel',[800:-100:100]);
set(get(axes1,'XLabel'),'String','','FontSize',26,'FontWeight','bold');
set(get(axes1,'YLabel'),'String','','FontSize',26,'FontWeight','bold');

colormap(gray)

%save to file
set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.
saveas(gcf, strcat('..\figs\',dataName), 'pdf') %Save figure  
saveas(gca, strcat('..\figs\',dataName, '.eps'),'psc2') %Save figure 