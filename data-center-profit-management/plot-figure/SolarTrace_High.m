%plot solar trace

Solar_High = load('.\data\realSolars.txt');

maxNum = max(Solar_High);
for i=1:1:size(Solar_High,2)
    if maxNum < Solar_High(1,i)
        maxNum = Solar_High(1,i)
    end
end

solar_loadFactor = 1;

X=[1:1:196];

save solar_High.mat



% Create figure
figure1 = figure;
%comment out, otherwise, figure will be normalized
%set(figure1,'units','normalized','outerposition',[0 0 1 1]);


% Create axes
%axis([xmin xmax ymin ymax])
axes1 = axes('Parent',figure1);
box(axes1,'on');
hold(axes1,'all');
set(axes1,'FontSize',30,'FontWeight','bold');



% p= plot(X(1,:),Solar(1,:)/3)
% set(p,'Color','g','LineWidth',3,'linestyle','-')
% set(p,'Marker','^','Markersize',12);

p = area(Solar_High(1:196,1)/maxNum*100 *solar_loadFactor,'FaceColor','g');

 
   
%change x-axis scale 
axis([1 196 0 100])
    
set(axes1,'XGrid','on','YGrid','off');
leg = legend(axes1,'show');
set(leg,'FontSize',30);
 
set(axes1,'YTick', [20,40,60,80,100], 'YTickLabel',{ 20,40,60,80,100},'XGrid','off','YGrid','off');



 set(axes1,'XTick',[1:48:196],'XTickLabel',{'day-1 00:00','day-1 12:00','day-2 00:00','day-2 12:00','day-3 00:00'},'XGrid','on');
 fix_xticklabels(gca,10,{'FontSize',30});

%legend
% legend('1K,Enumeration','1K,Approximation','5K,Enumeration','5K,Approximation','10K,Enumeration','10K,Approximation','100K,Enumeration','100K,Approximation')
legend('Solar Trace','location','northwest')

%set(get(axes1,'XLabel'),'String','Time','FontSize',30,'FontWeight','bold');
set(get(axes1,'YLabel'),'String','Solar Energy Units','FontSize',30,'FontWeight','bold');

%save to file
set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.
%saveas(gcf, 'SolarTrace_High', 'pdf') %Save figure
saveas(gcf, '.\figures\SolarTrace_High', 'pdf') %Save figure  
saveas(gca, strcat('.\figures\SolarTrace_High', '.eps'),'psc2') %Save figure 
    
    
 
    
    
    
    
    
    
    
    
    
    
    