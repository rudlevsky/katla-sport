import { Component, OnInit } from '@angular/core';
import { HiveSection } from '../models/hive-section';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveSectionService } from '../services/hive-section.service';

@Component({
  selector: 'app-hive-section-form',
  templateUrl: './hive-section-form.component.html',
  styleUrls: ['./hive-section-form.component.css']
})
export class HiveSectionFormComponent implements OnInit {

  hiveId = 0;
  hiveSection = new HiveSection(0, "", "", 0, false, "");
  existed = false;

  constructor(  
    private route: ActivatedRoute,
    private router: Router,
    private hiveSectionService: HiveSectionService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(s => {
      if (s['id'] === undefined){
        this.hiveSection.storeHiveId = s['store-hive-id'];
        return;
      }
      this.hiveSectionService.getHiveSection(s['id']).subscribe(h => {
        this.hiveSection = h;
        this.hiveSection.storeHiveId = s['store-hive-id'];
      });
      this.existed = true;
    })
  }

  navigateToHiveSections() {
    this.router.navigate([`/hive/${this.hiveSection.storeHiveId}/sections`]);
  }

  onCancel() {
    this.navigateToHiveSections();
  }

  onSubmit() {
    if (this.existed){
      this.hiveSectionService.updateHiveSection(this.hiveSection).subscribe(h => this.navigateToHiveSections());
    } else {
      this.hiveSectionService.addHiveSection(this.hiveSection).subscribe(h => this.navigateToHiveSections());
    }
  }

  onDelete() {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSection.id, true).subscribe(h => this.hiveSection.isDeleted = true);
  }

  onUndelete() {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSection.id, false).subscribe(h => this.hiveSection.isDeleted = false);
  }

  onPurge() {
    this.hiveSectionService.deleteHiveSection(this.hiveSection.id).subscribe(h => this.navigateToHiveSections());
  }
}
