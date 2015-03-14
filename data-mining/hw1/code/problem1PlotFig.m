%Build Figure



figure1 = figure;
set(figure1,'units','normalized','outerposition',[-0.5 -0.5 11 11]);

axes1 = axes('Parent',figure1);
box(axes1,'on');
hold(axes1,'all');
set(axes1,'FontSize',30,'FontWeight','bold');



[x y] = textread('..\data\z.txt');
 
    

 

plot(x(1:99,1),y(1:99,1), 'r.','MarkerSize', 20, 'linewidth' ,3 );
hold('on');
plot(x(100,1),y(100,1), 'b*','MarkerSize', 12, 'linewidth' ,3);
plot(x(101,1),y(101,1), 'go','MarkerSize', 12,'linewidth' ,3);
plot(x(102,1),y(102,1), 'ms','MarkerSize', 12,'linewidth' ,3);
plot(3.965,4.960,  'k>','MarkerSize', 12,'linewidth' ,3)

%set x range and y range
% axis([0 10 0 20])  

%set x tick and y tick
%set(axes1,'YTick',[0.2,0.4,0.6,0.8,1.0],'YTickLabel',{'20%','40%','60%','80%','100%'},'XGrid','on','YGrid','on');
%set(axes1,'XTick',[200,400,600,800,1000],'XTickLabel',{200,400,600,800,1000},'XGrid','on','YGrid','on');
 
%set grid on 
set(axes1,'XGrid','on','YGrid','on');
    
%set legend
 legend(axes1,'show','FontSize',10,'FontWeight','bold');
 l = legend('records 1-99','record 100','record 101','record 102', 'centroid');
set(l, 'Location','SouthEast')

%set x, y Label
set(get(axes1,'XLabel'),'String','x','FontSize',30,'FontWeight','bold');
set(get(axes1,'YLabel'),'String','y','FontSize',30,'FontWeight','bold');

%set title
%title('10000 Attackers, 1000 Proxies');

%save figure
set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.



saveas(gca, strcat('..\figs\visualizeZ', '.eps'),'psc2') %Save figure 
saveas(gcf, strcat('..\figs\visualizeZ'), 'pdf') %Save figure






