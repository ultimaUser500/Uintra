import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AddButtonService } from '../../main-layout/left-navigation/components/my-links/add-button.service';

@Component({
  selector: 'home-page',
  templateUrl: './home-page.html',
  styleUrls: ['./home-page.less'],
  encapsulation: ViewEncapsulation.None
})
export class HomePage implements OnInit {

  data: any;
  latestActivities: any;
  otherPanels: any;
  constructor(
    private route: ActivatedRoute,
    private addButtonService: AddButtonService
  ) {
    this.route.data.subscribe(data => {
      this.data = data;
      this.addButtonService.setPageId(data.id);
    });
  }

  ngOnInit(): void {
    if (this.data.panels) {
      this.latestActivities = this.data.panels.get().filter(p => p.data.contentTypeAlias === 'latestActivitiesPanel')[0];
      this.otherPanels = this.data.panels.get().filter(p => p.data.contentTypeAlias !== 'latestActivitiesPanel');
    }
  }
}
