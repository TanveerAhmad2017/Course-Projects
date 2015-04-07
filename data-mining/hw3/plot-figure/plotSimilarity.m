%Goal: visualize the similarity matrix of different cluster results
function plotSimilarity(dataName)


data1 = load(strcat('..\processed-data\',dataName,'.txt'));
sortedData1 =sortrows(data1, 7);
sortedData1 = sortedData1(:,2:6);

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

set(gca,'FontSize', 26);

%set x, y Label
set(get(axes1,'XLabel'),'String','','FontSize',26,'FontWeight','bold');
set(get(axes1,'YLabel'),'String','','FontSize',26,'FontWeight','bold');

colormap(gray)

%save to file
set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.
saveas(gcf, strcat('..\figs\',dataName), 'pdf') %Save figure  
saveas(gca, strcat('..\figs\',dataName, '.eps'),'psc2') %Save figure 

end