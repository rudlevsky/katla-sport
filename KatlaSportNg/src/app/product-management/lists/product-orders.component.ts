import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product-category-list',
  templateUrl: './product-orders.component.html',
  styleUrls: ['./product-category-list.component.css']
})
export class ProductOrdersComponent implements OnInit {
  categoryId: number;
  orderCount: number;
  productId: number;

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.params.subscribe(p => {this.categoryId = p['categoryId'];
    this.orderCount = p['orderCount'];
    this.productId = p['productId'];
  });
  }

  navigateTo() {
    this.router.navigate(['/products']);
  }

  onSave() {
    if (this.orderCount < 11 && !(this.orderCount <= 0)) {
      this.productService.getProduct(this.productId).subscribe(p => {
        
        p.name = "PRODUCTNAME";
        p.code = "TPROD";

        for(let i = 0; i < this.orderCount; i++) {
          this.productService.addProduct(p).subscribe(p => this.navigateTo());
        }
      });
    }
  }
}
